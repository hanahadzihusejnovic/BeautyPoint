using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BeautyPoint.Repositories
{
    public class ProductReviewRepository : GenericRepository<ProductReview>, IProductReviewRepository
    {
        private readonly DatabaseContext _context;

        public ProductReviewRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ProductReview>> GetReviewsByProductId(int productId)
        {
            return await _context.ProductReviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .ToListAsync();
        }

        public async Task<double> GetAverageRating(int productId)
        {
            var reviews = await _context.ProductReviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<ProductReview?> GetReviewByIdAsync(int reviewId)
        {
            return await _context.ProductReviews.FindAsync(reviewId);
        }

        public async Task<bool> UserHasReviewedProductAsync(string userId, int productId)
        {
            return await _context.ProductReviews.AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        }

    }
}
