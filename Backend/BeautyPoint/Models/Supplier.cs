using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = default!;
        public string ContactName { get; set; } = default!;
        public string ContactEmail { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Address { get; set; } = default!;

        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
    }
}
