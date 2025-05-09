using BeautyPoint.Models;
using BeautyPoint.ViewModels;

namespace BeautyPoint.ViewModels
{
    public class OrderVModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public int PaymentId {  get; set; }
        public string PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId {  get; set; }
        public ICollection<OrderItemVModel> OrderItems { get; set; }
    }
}
