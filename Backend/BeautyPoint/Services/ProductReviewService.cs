using BeautyPoint.Data;
using BeautyPoint.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BeautyPoint.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly DatabaseContext _context;

        public ProductReviewService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task AddReviewAsync(ProductReview review)
        {
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> UpdateReviewAsync(int reviewId, string userId, int rating, string comment)
        {
            var review = await _context.ProductReviews.FindAsync(reviewId);

            if (review == null || review.UserId != userId)
                return false; 

            review.Rating = rating;
            review.Comment = comment;
            review.ReviewDate = DateTime.UtcNow;

            _context.ProductReviews.Update(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, string userId)
        {
            var review = await _context.ProductReviews.FindAsync(reviewId);

            if (review == null || review.UserId != userId)
                return false; 

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
