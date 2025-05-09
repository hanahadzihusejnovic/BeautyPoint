using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;

namespace BeautyPoint.Services
{
    public class SavedItemService : ISavedItemService
    {
        private readonly ISavedItemsRepository _savedItemsRepository;

        public SavedItemService(ISavedItemsRepository savedItemsRepository)
        {
            _savedItemsRepository = savedItemsRepository;
        }

        public async Task SaveForLater(string userId, int productId, int quantity)
        {
            await _savedItemsRepository.SaveForLater(userId, productId, quantity);
        }

        public async Task RemoveFromSaved(string userId, int productId)
        {
            await _savedItemsRepository.RemoveFromSaved(userId, productId);
        }

        public async Task RemoveFromCart(string userId, int productId)
        {
            await _savedItemsRepository.RemoveFromCart(userId, productId);
        }

        public async Task<List<SavedItem>> GetSavedItemsByUser(string userId)
        {
            return await _savedItemsRepository.GetSavedItemsByUser(userId);
        }

        public async Task MoveToCart(string userId, int productId)
        {
            await _savedItemsRepository.MoveToCart(userId, productId);
        }
    }
}
