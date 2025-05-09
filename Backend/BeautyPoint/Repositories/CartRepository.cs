using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautyPoint.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly DatabaseContext _context;

        public CartRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<CartItem> AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await GetCartAsync(userId);
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                throw new Exception("Proizvod nije pronađen.");
            }

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.Price = cartItem.Quantity * product.Price; // ✅ Ispravljeno: Ažurira se cijena 
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = quantity * product.Price // ✅ Ispravljeno: Postavlja se ukupna cijena
                };
                cart.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return cartItem;
        }


        public async Task<CartItem> UpdateCartAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Product) // Učitavamo i Product da bismo dobili cijenu
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                cartItem.Price = cartItem.Product.Price * quantity; // Ažuriramo cijenu
                await _context.SaveChangesAsync();
            }
            return cartItem;
        }


        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }
        }
    }

}
