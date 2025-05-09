namespace BeautyPoint.DTOs
{
    public class ProductReviewCreateDto
    {
        public string UserId { get; set; } = default!;
        public int ProductId { get; set; }
        public int ProductRating { get; set; }
        public string ProductComment { get; set; } = default!;
    }
}
