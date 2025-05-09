using BeautyPoint.Models;
using BeautyPoint.ViewModels;

namespace BeautyPoint.ViewModels
{
    public class ProductReviewVModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductRating { get; set; }
        public string ProductComment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
