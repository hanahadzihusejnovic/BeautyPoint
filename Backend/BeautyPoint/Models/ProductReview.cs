using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class ProductReview
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public int Rating { get; set; }
        public string Comment { get; set; } = default!;
        public DateTime ReviewDate { get; set; }
    }
}
