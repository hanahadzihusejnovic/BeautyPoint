using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class Treatment
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = default!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Description { get; set; } = default!;

        public int TreatmentCategoryId { get; set; }
        public TreatmentCategory TreatmentCategory { get; set; } = default!;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<TreatmentReview> TreatmentReviews { get; set; } = new List<TreatmentReview>();
        public ICollection<ServicePackageTreatment> ServicePackageTreatments { get; set; } = new List<ServicePackageTreatment>();
    }
}
