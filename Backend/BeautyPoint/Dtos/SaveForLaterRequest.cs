﻿namespace BeautyPoint.Dtos
{
    public class SaveForLaterRequest
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
