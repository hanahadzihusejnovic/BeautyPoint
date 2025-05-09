using BeautyPoint.Models;

namespace BeautyPoint.ViewModels
{
    public class ServicePackageTreatmentVModel
    {
        public int Id { get; set; }
        public int ServicePackageId { get; set; }
        public string ServicePackageName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }

    }
}
