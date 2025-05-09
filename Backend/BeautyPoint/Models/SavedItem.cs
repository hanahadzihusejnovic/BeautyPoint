namespace BeautyPoint.Models
{
    public class SavedItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; } = default!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; }
    }
}
