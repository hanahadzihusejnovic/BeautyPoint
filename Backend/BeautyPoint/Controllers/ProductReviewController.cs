using AutoMapper;
using BeautyPoint.Data;
using BeautyPoint.DTOs;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.SearchObjects;
using BeautyPoint.Services;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BeautyPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductReviewController : ControllerBase
    {
        private readonly IGenericRepository<ProductReview> _productReviewRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;
        private readonly IProductReviewService _productReviewService;


        public ProductReviewController(
            IGenericRepository<ProductReview> productReviewRepository,
            IMapper mapper,
            DatabaseContext databaseContext,
            IProductReviewService productReviewService)
        {
            _productReviewRepository = productReviewRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
            _productReviewService = productReviewService;
        }

        [HttpPost("add")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> AddReview([FromBody] ProductReviewCreateDto reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest("Podaci o recenziji nisu validni.");
            }

            var newReview = new ProductReview
            {
                UserId = reviewDto.UserId,
                ProductId = reviewDto.ProductId,
                Rating = reviewDto.ProductRating,
                Comment = reviewDto.ProductComment,
                ReviewDate = DateTime.UtcNow
            };

            await _productReviewService.AddReviewAsync(newReview);
            return Ok(new { message = "Recenzija uspešno dodana!" });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetProductReview(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var productReview = await _productReviewRepository.GetByIdAsync(id, "Product,User");
            if (productReview == null) return NotFound();

            if (userRole == "Client" && productReview.UserId != userId) return Forbid();

            return Ok(_mapper.Map<ProductReviewVModel>(productReview));
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _databaseContext.ProductReviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.Product)
                .Include(r => r.User)
                .ToListAsync();

            if (!reviews.Any()) return Ok(new List<ProductReviewVModel>()); 

            var response = _mapper.Map<List<ProductReviewVModel>>(reviews);
            return Ok(response);
        }

        [HttpGet("product/{productId}/average-rating")]
        public async Task<IActionResult> GetAverageRating(int productId)
        {
            var averageRating = await _databaseContext.ProductReviews
                .Where(r => r.ProductId == productId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;

            return Ok(new { averageRating });
        }

        [HttpPut("update/{reviewId}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] ProductReviewCreateDto reviewDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _productReviewService.UpdateReviewAsync(reviewId, userId, reviewDto.ProductRating, reviewDto.ProductComment);

            if (!success)
                return BadRequest("Neuspelo ažuriranje recenzije.");

            return Ok(new { message = "Recenzija uspešno ažurirana!" });
        }

        [HttpDelete("delete/{reviewId}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _productReviewService.DeleteReviewAsync(reviewId, userId);

            if (!success)
                return BadRequest("Neuspelo brisanje recenzije.");

            return Ok(new { message = "Recenzija uspešno obrisana!" });
        }

    }
}


