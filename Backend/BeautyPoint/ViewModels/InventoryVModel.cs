namespace BeautyPoint.ViewModels
{
    public class InventoryVModel
    {
        public int Id {  get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int QuantityInStock { get; set; }
    }
}
