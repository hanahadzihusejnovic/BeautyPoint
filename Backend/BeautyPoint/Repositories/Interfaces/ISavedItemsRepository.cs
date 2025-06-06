﻿using BeautyPoint.Models;

namespace BeautyPoint.Repositories.Interfaces
{
    public interface ISavedItemsRepository
    {
        Task SaveForLater(string userId, int productId, int quantity);
        Task RemoveFromSaved(string userId, int productId);
        Task RemoveFromCart(string userId, int productId);
        Task<List<SavedItem>> GetSavedItemsByUser(string userId);
        Task MoveToCart(string userId, int productId);
    }

}
