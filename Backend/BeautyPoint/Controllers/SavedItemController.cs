using BeautyPoint.Dtos;
using BeautyPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeautyPoint.Controllers
{
    [Route("api/saved-items")]
    [ApiController]
    public class SavedItemsController : ControllerBase
    {
        private readonly ISavedItemService _savedItemsService;

        public SavedItemsController(ISavedItemService savedItemsService)
        {
            _savedItemsService = savedItemsService;
        }

        [HttpPost("save-for-later")]
        public async Task<IActionResult> SaveForLater([FromBody] SaveForLaterRequest request)
        {
            await _savedItemsService.SaveForLater(request.UserId, request.ProductId, request.Quantity);
            return Ok(new { message = "Proizvod spremljen za kasnije." });
        }

        [HttpGet("get-saved-items/{userId}")]
        public async Task<IActionResult> GetSavedItems(string userId)
        {
            var savedItems = await _savedItemsService.GetSavedItemsByUser(userId);
            return Ok(savedItems);
        }

        [HttpDelete("remove/{userId}/{productId}")]
        public async Task<IActionResult> RemoveFromSaved(string userId, int productId)
        {
            await _savedItemsService.RemoveFromSaved(userId, productId);
            return Ok(new { message = "Proizvod uklonjen iz spremljenih." });
        }

        [HttpDelete("removeCart/{userId}/{productId}")]
        public async Task<IActionResult> RemoveFromCart(string userId, int productId)
        {
            await _savedItemsService.RemoveFromCart(userId, productId);
            return Ok(new { message = "Proizvod uklonjen iz korpe." });
        }

        [HttpPost("move-to-cart")]
        public async Task<IActionResult> MoveToCart([FromBody] MoveToCartRequest request)
        {
            await _savedItemsService.MoveToCart(request.UserId, request.ProductId);
            return Ok(new { message = "Proizvod premješten u korpu." });
        }
    }

}
