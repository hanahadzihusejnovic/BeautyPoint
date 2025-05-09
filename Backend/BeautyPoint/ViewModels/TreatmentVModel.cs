using BeautyPoint.Models;

namespace BeautyPoint.ViewModels
{
    public class TreatmentVModel
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } 
        public decimal TreatmentPrice { get; set; }
        public string TreatmentDescription { get; set; } 
        public int TreatmentCategoryId { get; set; }
        public string TreatmentCategoryName { get; set; }
        public ICollection<TreatmentReviewVModel> TreatmentReviews { get; set; } = new List<TreatmentReviewVModel>();
        public ICollection<ServicePackageTreatmentVModel> ServicePackageTreatments { get; set; } = new List<ServicePackageTreatmentVModel>();
    }
}
