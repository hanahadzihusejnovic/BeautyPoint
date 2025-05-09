namespace BeautyPoint.Dtos
{
    public class ConfirmPaymentDto
    {
        public int OrderId { get; set; }
        public string PaymentIntentId { get; set; } = default!;
    }
}
