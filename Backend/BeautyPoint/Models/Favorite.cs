using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        public User User { get; set; } = default!;

        public int ProductId { get; set; } 
        public Product Product { get; set; } = default!;
    }
}
