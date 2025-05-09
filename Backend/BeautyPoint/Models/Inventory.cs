using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class Inventory
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int QuantityInStock { get; set; }
    }
}
