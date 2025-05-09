using BeautyPoint.Dtos;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeautyPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

      
        [HttpPost("add")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var cartItem = await _cartRepository.AddToCartAsync(request.UserId, request.ProductId, request.Quantity);
            return Ok(cartItem);
        }

        [HttpPut("update/{cartItemId}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> UpdateCart(int cartItemId, [FromBody] UpdateCartRequest request)
        {
            if (request == null || request.Quantity <= 0)
            {
                return BadRequest("Neispravan unos količine.");
            }

            var updatedItem = await _cartRepository.UpdateCartAsync(cartItemId, request.Quantity);

            if (updatedItem == null)
            {
                return NotFound("Stavka u korpi nije pronađena.");
            }

            return Ok(updatedItem);
        }


        [HttpDelete("remove/{cartItemId}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            await _cartRepository.RemoveFromCartAsync(cartItemId);
            return NoContent();
        }

     
        [HttpGet("view/{userId}")]
        [Authorize(Roles = "Client, Admin")]
        public async Task<IActionResult> GetCart(string userId)
        {
            var cart = await _cartRepository.GetCartAsync(userId);
            if (cart == null)
            {
                return Ok(new List<CartItem>());
            }

            return Ok(cart);
        }

        [HttpDelete("clear/{userId}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            await _cartRepository.ClearCartAsync(userId);
            return NoContent();
        }
    }

}
