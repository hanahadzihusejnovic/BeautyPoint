using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautyPoint.Repositories
{
    public class SavedItemsRepository : ISavedItemsRepository
    {
        private readonly DatabaseContext _context;

        public SavedItemsRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task SaveForLater(string userId, int productId, int quantity)
        {
            var existingSavedItem = await _context.SavedItems
       .                                  FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

            if (existingSavedItem == null)
            {
                var savedItem = new SavedItem { UserId = userId, ProductId = productId, Quantity = quantity};
                _context.SavedItems.Add(savedItem);
            }
            else
            {
                existingSavedItem.Quantity += quantity; // Ažuriranje količine ako već postoji

            }
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromSaved(string userId, int productId)
        {
            var item = await _context.SavedItems
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
            if (item != null)
            {
                _context.SavedItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCart(string userId, int productId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Cart.UserId == userId && ci.ProductId == productId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<List<SavedItem>> GetSavedItemsByUser(string userId)
        {
            return await _context.SavedItems
                .Include(s => s.Product)
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task MoveToCart(string userId, int productId)
        {
            // Prvo provjeri ima li korisnik proizvod u SavedItems
            var savedItem = await _context.SavedItems
                .Include(s => s.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

            if (savedItem == null || savedItem.Product == null)
            {
                throw new Exception("Proizvod nije pronađen među spremljenim stavkama.");
            }

            // Provjeri da proizvod nije već u CartItems
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Cart.UserId == userId && ci.ProductId == productId);

            if (existingCartItem != null)
            {
                // Ako proizvod već postoji u korpi, nemoj ga dodavati ponovo
                return;
            }

            // Pronađi korisnikovu korpu
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // Sačuvaj kako bi dobili CartId
            }

            // Ukloni proizvod iz SavedItems
            _context.SavedItems.Remove(savedItem);

            // Dodaj u CartItems
            _context.CartItems.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = savedItem.Quantity, 
                Price = savedItem.Product.Price * savedItem.Quantity // Preuzmi cijenu iz proizvoda
            });

            await _context.SaveChangesAsync();
        }

    }
}
