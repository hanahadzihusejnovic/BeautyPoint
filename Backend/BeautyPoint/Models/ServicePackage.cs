using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class ServicePackage
    {
        public int Id { get; set; }
        public string PackageName { get; set; } = default!;
        public string Description { get; set; } = default!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public ICollection<ServicePackageTreatment> ServicePackageTreatments { get; set; } = new List<ServicePackageTreatment>();
    }
}
