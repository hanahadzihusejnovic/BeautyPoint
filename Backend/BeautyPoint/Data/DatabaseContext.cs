using BeautyPoint.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;


namespace BeautyPoint.Data
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ProductSupplier> ProductSuppliers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<TreatmentCategory> TreatmentCategories { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<TreatmentReview> TreatmentReviews { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<ServicePackage> ServicePackages { get; set; }
        public DbSet<ServicePackageTreatment> ServicePackageTreatments { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<SavedItem> SavedItems { get; set; }

    }
}
