using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        public User User { get; set; } = default!;

        public int TreatmentId { get; set; }
        public Treatment Treatment { get; set; } = default!;

        public DateTime ReservationDate { get; set; } = default!; 
        public string Status { get; set; } = default!;

    }
}
