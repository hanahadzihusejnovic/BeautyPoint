
using BeautyPoint.Data;
using BeautyPoint.Dtos;
using BeautyPoint.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace BeautyPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public CheckoutController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutModel model, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                                      .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null) return BadRequest("User not found.");

            var cart = await _context.Carts
                                      .Include(c => c.CartItems)
                                      .FirstOrDefaultAsync(c => c.UserId == model.UserId);

            if (cart == null || !cart.CartItems.Any())
                return BadRequest("Your cart is empty.");

            decimal totalPrice = cart.CartItems.Sum(c => c.Price * c.Quantity);
            var orderItems = cart.CartItems.Select(c => new OrderItem
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                Price = c.Price
            }).ToList();

            var order = new Order
            {
                UserId = model.UserId,
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                Status = "Pending",
                OrderItems = orderItems
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync(cancellationToken);

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new { OrderId = order.Id, TotalPrice = totalPrice });
        }

    }
}

