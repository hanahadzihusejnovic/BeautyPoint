using BeautyPoint.ViewModels;

namespace BeautyPoint.ViewModels
{
    public class TreatmentReviewVModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int TreatmentRating { get; set; }
        public string TreatmentComment { get; set; } 
        public DateTime ReviewDate { get; set; }
    }
}
