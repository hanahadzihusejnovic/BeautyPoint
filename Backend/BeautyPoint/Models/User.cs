using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyPoint.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = default!;
        public string City { get; set; } = default!;
        public UserRole Role { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<TreatmentReview> TreatmentReviews { get; set; } = new List<TreatmentReview>();
        public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();
    }
}

public enum UserRole
{
    Client,
    Employee,
    Admin
}