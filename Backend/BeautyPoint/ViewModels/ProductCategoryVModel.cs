using BeautyPoint.Models;

namespace BeautyPoint.ViewModels
{
    public class ProductCategoryVModel
    {
        public int Id { get; set; }
        public string ProductCategoryName { get; set; }

        public ICollection<ProductVModel> Products { get; set; }
    }
}
