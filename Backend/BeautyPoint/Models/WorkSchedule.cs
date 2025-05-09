using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class WorkSchedule
    {
        public int Id { get; set; }

        public string EmployeeId { get; set; } = default!;
        public User Employee { get; set; } = default!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
