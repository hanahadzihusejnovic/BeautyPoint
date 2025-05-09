using AutoMapper;
using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.SearchObjects;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BeautyPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;
        private readonly IGenericRepository<Payment> _paymentRepository;

        public OrderController(
            IGenericRepository<Order> orderRepository,
            IMapper mapper,
            DatabaseContext databaseContext,
            IGenericRepository<Payment> paymentRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
            _paymentRepository = paymentRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
       
        public async Task<IActionResult> CreateOrder(OrderVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var order = _mapper.Map<Order>(model);

            var user = await _databaseContext.Users
                                              .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            order.User = user;

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync(cancellationToken); 

            var payment = await _databaseContext.Payments
                                                 .FirstOrDefaultAsync(p => p.OrderId == order.Id);

            if (payment != null)
            {
                payment.PaymentDate = order.OrderDate; 
                _databaseContext.Payments.Update(payment);
                await _databaseContext.SaveChangesAsync(cancellationToken);
            }

            return Ok(_mapper.Map<OrderVModel>(order));
        }




        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var order = await _orderRepository.GetByIdAsync(id, "User,Payment,OrderItems,OrderItems.Product");

            if (order == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && order.UserId.ToString() != userId)
            {
                return Forbid();
            }

            var orderVModel = _mapper.Map<OrderVModel>(order);
            return Ok(orderVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search, CancellationToken cancellationToken = default)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var ordersQuery = _databaseContext.Orders
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            if (userRole == "Client")
            {
                ordersQuery = ordersQuery.Where(o => o.UserId.ToString() == userId);
            }

            var totalCount = await ordersQuery.CountAsync(cancellationToken);

            var orders = await ordersQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            var ordersList = _mapper.Map<List<OrderVModel>>(orders);

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            return Ok(ordersList);
        }


        [HttpPut("{id}")]
        [Authorize (Roles = "Client,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderVModel model, CancellationToken cancellationToken = default)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && order.UserId.ToString() != userId)
            {
                return Forbid();
            }

            _mapper.Map(model, order);

            var user = await _databaseContext.Users
                                             .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }
            order.User = user;

            var payment = await _databaseContext.Payments
                                                .FirstOrDefaultAsync(p => p.Id == model.PaymentId);

            if (payment == null)
            {
                return BadRequest("Payment does not exist.");
            }
            order.Payment = payment;

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && order.UserId.ToString() != userId)
            {
                return Forbid();
            }

            await _orderRepository.DeleteAsync(order);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
