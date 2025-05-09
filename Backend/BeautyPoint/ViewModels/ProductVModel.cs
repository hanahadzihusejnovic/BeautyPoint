using BeautyPoint.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeautyPoint.ViewModels
{
    public class ProductVModel
    {
        public int Id { get; set; }

        [FromForm]
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; }

        [FromForm]
        [Required(ErrorMessage = "Product price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than zero.")]
        public decimal ProductPrice { get; set; }

        [FromForm]
        [Required(ErrorMessage = "Product description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string ProductDescription { get; set; }

        [FromForm]
        [Required(ErrorMessage = "Product category is required.")]
        public int ProductCategoryId { get; set; }

        public string ProductCategoryName { get; set; }

        public string ImagePath { get; set; }

        [FromForm]
        [Required(ErrorMessage = "Volume is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Volume must be a positive number.")]
        public int Volume { get; set; }

        [FromForm]
        public IFormFile ProductImage { get; set; }

        public ICollection<ProductReviewVModel> ProductReviews { get; set; } = new List<ProductReviewVModel>();
    }
}
