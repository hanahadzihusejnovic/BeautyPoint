using BeautyPoint.Models;

namespace BeautyPoint.Repositories.Interfaces
{
    public interface IProductReviewRepository : IGenericRepository<ProductReview>
    {
        Task<List<ProductReview>> GetReviewsByProductId(int productId);
        Task<double> GetAverageRating(int productId);
        Task<ProductReview?> GetReviewByIdAsync(int reviewId);
        Task<bool> UserHasReviewedProductAsync(string userId, int productId);

    }
}