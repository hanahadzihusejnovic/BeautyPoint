using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class TreatmentCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = default!; 

        public ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
    }
}
