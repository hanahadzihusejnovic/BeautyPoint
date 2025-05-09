using BeautyPoint.Models;

namespace BeautyPoint.ViewModels
{
    public class ServicePackageVModel
    {
        public int Id { get; set; }
        public string PackageName { get; set; } 
        public string PackageDescription { get; set; }
        public decimal PackagePrice { get; set; }

        public ICollection<ServicePackageTreatmentVModel> ServicePackageTreatments { get; set; }
    }
}
