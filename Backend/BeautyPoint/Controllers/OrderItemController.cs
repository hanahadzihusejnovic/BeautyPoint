
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

namespace BeautyPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IGenericRepository<OrderItem> _orderItemRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;

        public OrderItemController(
            IGenericRepository<OrderItem> orderItemRepository,
            IMapper mapper,
            DatabaseContext databaseContext)
        {
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CreateOrderItem(OrderItemVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var orderItem = _mapper.Map<OrderItem>(model);

            var product = await _databaseContext.Products
                                           .FirstOrDefaultAsync(p => p.Id == model.ProductId);

            if (product == null)
            {
                return BadRequest("Product does not exist.");
            }
            orderItem.Product = product;

            var order = await _databaseContext.Orders
                                           .Include(o => o.User)
                                           .FirstOrDefaultAsync(o => o.Id == model.OrderId);

            if (order == null)
            {
                return BadRequest("Order does not exist.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (order.UserId.ToString() != userId)
            {
                return Forbid();
            }

            orderItem.Order = order;

            await _orderItemRepository.AddAsync(orderItem);
            await _orderItemRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<OrderItemVModel>(orderItem));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetOrderItem(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orderItem = await _orderItemRepository.GetByIdAsync(id, "Product,Order,Order.User");

            if (orderItem == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && orderItem.Order.UserId.ToString() != userId)
            {
                return Forbid();
            }

            var orderItemVModel = _mapper.Map<OrderItemVModel>(orderItem);
            return Ok(orderItemVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orderItemsQuery = await _orderItemRepository.GetAllAsync(includeProperties: "Product,Order,Order.User");

            if (userRole == "Client")
            {
                orderItemsQuery = orderItemsQuery.Where(oi => oi.Order.UserId.ToString() == userId);
            }

            var totalCount = orderItemsQuery.Count();

            var orderItems = orderItemsQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var orderItemsList = _mapper.Map<List<OrderItemVModel>>(orderItems);
            return Ok(orderItemsList);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderItemVModel model, CancellationToken cancellationToken = default)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orderItem = await _orderItemRepository.GetByIdAsync(id, "Order,Order.User");

            if (orderItem == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && orderItem.Order.UserId.ToString() != userId)
            {
                return Forbid();
            }

            _mapper.Map(model, orderItem);

            var product = await _databaseContext.Products
                                           .FirstOrDefaultAsync(p => p.Id == model.ProductId);

            if (product == null)
            {
                return BadRequest("Product does not exist.");
            }
            orderItem.Product = product;

            var order = await _databaseContext.Orders
                                           .Include(o => o.User)
                                           .FirstOrDefaultAsync(o => o.Id == model.OrderId);

            if (order == null)
            {
                return BadRequest("Order does not exist.");
            }
            orderItem.Order = order;

            await _orderItemRepository.UpdateAsync(orderItem);
            await _orderItemRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orderItem = await _orderItemRepository.GetByIdAsync(id, "Order,Order.User");

            if (orderItem == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && orderItem.Order.UserId.ToString() != userId)
            {
                return Forbid();
            }

            await _orderItemRepository.DeleteAsync(orderItem);
            await _orderItemRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
