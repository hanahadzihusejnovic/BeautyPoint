using BeautyPoint.Models;

namespace BeautyPoint.ViewModels
{
    public class TreatmentCategoryVModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }

        public ICollection<TreatmentVModel> Treatments { get; set; }
    }
}
