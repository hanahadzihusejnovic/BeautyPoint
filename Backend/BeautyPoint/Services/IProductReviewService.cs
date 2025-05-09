using BeautyPoint.Models;

namespace BeautyPoint.Services
{
    public interface IProductReviewService
    {
        Task AddReviewAsync(ProductReview review);
        Task<bool> UpdateReviewAsync(int reviewId, string userId, int rating, string comment);
        Task<bool> DeleteReviewAsync(int reviewId, string userId);

    }
}
