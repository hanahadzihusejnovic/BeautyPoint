﻿namespace BeautyPoint.ViewModels
{
    public class PaymentVModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus {  get; set; }
        public string TransactionId {  get; set; }
    }
}
