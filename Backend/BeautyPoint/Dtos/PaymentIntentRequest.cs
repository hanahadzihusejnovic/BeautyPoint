namespace BeautyPoint.Dtos
{
    public class PaymentIntentRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}
