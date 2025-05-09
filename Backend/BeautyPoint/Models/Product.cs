using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Description { get; set; } = default!;

        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; } = default!;

        public Inventory Inventory { get; set; } = default!;
        public string ImagePath { get; set; } 
        public int Volume { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
    }
}

