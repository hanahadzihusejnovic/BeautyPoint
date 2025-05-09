using AutoMapper;
using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.SearchObjects;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

using Stripe;
using Microsoft.Extensions.Options;
using BeautyPoint.Configurations;
using BeautyPoint.Dtos;


namespace BeautyPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;

        private readonly StripeSettings _stripeSettings;

        public PaymentController(
            IGenericRepository<Payment> paymentRepository,
            IMapper mapper,
            DatabaseContext databaseContext,
            IOptions<StripeSettings> stripeSettings)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
            _stripeSettings = stripeSettings.Value;

            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePayment(PaymentVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var payment = _mapper.Map<Payment>(model);

            var order = await _databaseContext.Orders
                                           .FirstOrDefaultAsync(o => o.Id == model.OrderId);

            if (order == null)
            {
                return BadRequest("Order does not exist.");
            }
            payment.Order = order;

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<PaymentVModel>(payment));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetPayment(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var payment = await _paymentRepository.GetByIdAsync(id, "Order");

            if (payment == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && payment.Order.UserId.ToString() != userId)
            {
                return Forbid();
            }

            var paymentVModel = _mapper.Map<PaymentVModel>(payment);
            return Ok(paymentVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var paymentsQuery = _databaseContext.Payments
                .Include(p => p.Order)
                .ThenInclude(o => o.User)
                .AsQueryable();

            if (userRole == "Client")
            {
                paymentsQuery = paymentsQuery.Where(p => p.Order.UserId.ToString() == userId);
            }

            var totalCount = await paymentsQuery.CountAsync();

            var payments = await paymentsQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            var paymentsVModels = _mapper.Map<List<PaymentVModel>>(payments);

            var pagedResponse = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize,
                Payments = paymentsVModels
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(new { totalCount, search.PageSize }));

            return Ok(pagedResponse);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] PaymentVModel model, CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            _mapper.Map(model, payment);

            var order = await _databaseContext.Orders
                                           .FirstOrDefaultAsync(o => o.Id == model.OrderId);

            if (order == null)
            {
                return BadRequest("Order does not exist.");
            }
            payment.Order = order;

            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            await _paymentRepository.DeleteAsync(payment);
            await _paymentRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpPost("create-payment-intent")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentIntentRequest model)
        {
            if (model == null || model.Amount <= 0)
            {
                return BadRequest("Invalid payment data.");
            }

            try
            {
                if (model.OrderId > 0)
                {
                    var order = await _databaseContext.Orders
                                                       .FirstOrDefaultAsync(o => o.Id == model.OrderId);

                    if (order == null)
                    {
                        return BadRequest("Order does not exist.");
                    }
                }

                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(model.Amount * 100), 
                    Currency = "usd",
                    Metadata = new Dictionary<string, string>
            {
                { "OrderId", model.OrderId.ToString() }
            }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                return Ok(new
                {
                    ClientSecret = paymentIntent.ClientSecret
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating payment intent: {ex.Message}");
            }
        }


        [HttpPost("confirm-payment")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentDto model, CancellationToken cancellationToken = default)
        {
            if (model == null || string.IsNullOrEmpty(model.PaymentIntentId))
            {
                return BadRequest("Invalid payment data.");
            }

            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(model.PaymentIntentId);

                if (paymentIntent == null || paymentIntent.Status != "succeeded")
                {
                    return BadRequest("Payment was not successful.");
                }

                var order = await _databaseContext.Orders
                                                   .FirstOrDefaultAsync(o => o.Id == model.OrderId);

                if (order == null)
                {
                    return BadRequest("Order does not exist.");
                }

                var payment = new Payment
                {
                    OrderId = model.OrderId,
                    PaymentDate = DateTime.UtcNow,
                    Amount = (decimal)(paymentIntent.Amount / 100.0),
                    PaymentMethod = paymentIntent.PaymentMethodTypes.FirstOrDefault() ?? "Unknown",
                    TransactionId = paymentIntent.Id,
                    PaymentStatus = paymentIntent.Status
                };

                await _paymentRepository.AddAsync(payment);
                await _paymentRepository.SaveChangesAsync(cancellationToken);

                order.Status = "Paid";
                _databaseContext.Orders.Update(order);
                await _databaseContext.SaveChangesAsync();

                return Ok(new { Message = "Payment confirmed and saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error confirming payment: {ex.Message}");
            }
        }



    }
}
