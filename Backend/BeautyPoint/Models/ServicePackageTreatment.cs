using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class ServicePackageTreatment
    {
        public int Id { get; set; }

        public int ServicePackageId { get; set; }
        public ServicePackage ServicePackage { get; set; } = default!;

        public int TreatmentId { get; set; }
        public Treatment Treatment { get; set; } = default!;
    }
}
