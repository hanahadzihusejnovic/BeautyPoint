using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class TreatmentReview
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        public User User { get; set; } = default!;

        public int TreatmentId { get; set; }
        public Treatment Treatment { get; set; } = default!;

        public int Rating { get; set; }
        public string Comment { get; set; } = default!;
        public DateTime ReviewDate { get; set; }
    }
}
