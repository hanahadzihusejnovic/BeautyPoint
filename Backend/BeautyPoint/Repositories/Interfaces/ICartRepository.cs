using BeautyPoint.Models;

namespace BeautyPoint.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartAsync(string userId);
        Task<CartItem> AddToCartAsync(string userId, int productId, int quantity);
        Task<CartItem> UpdateCartAsync(int cartItemId, int quantity);
        Task RemoveFromCartAsync(int cartItemId);
        Task ClearCartAsync(string userId);
    }
}
