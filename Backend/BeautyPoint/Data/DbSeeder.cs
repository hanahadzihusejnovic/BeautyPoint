using BeautyPoint.Models;
using Microsoft.AspNetCore.Identity;

namespace BeautyPoint.Data
{
    public class DbSeeder
    {
        public static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            if (!userManager.Users.Any())
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@adminBeautyPoint.com",
                    FirstName = "Admin",
                    LastName = "Alastname",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    Address = "Address1",
                    City = "City1",
                    Role = UserRole.Admin
                };

                await userManager.CreateAsync(admin, "Admin123!");

                var employee = new User
                {
                    UserName = "employee",
                    Email = "employee@employeeBeautyPoint.com",
                    FirstName = "Employee",
                    LastName = "Elastname",
                    DateOfBirth = new DateTime(1990, 2, 2),
                    Address = "Address2",
                    City = "City2",
                    Role = UserRole.Employee
                };

                await userManager.CreateAsync(employee, "Employee123!");

                var client = new User
                {
                    UserName = "client",
                    Email = "client@gmail.com",
                    FirstName = "Client",
                    LastName = "Clastname",
                    DateOfBirth = new DateTime(1995, 3, 3),
                    Address = "Address3",
                    City = "City3",
                    Role = UserRole.Client
                };

                await userManager.CreateAsync(client, "Client123!");
            }
        }

        public static async Task SeedProductCategoriesAsync(DatabaseContext context)
        {
            if (!context.ProductCategories.Any())
            {
                var categories = new List<ProductCategory>
                {
                    new ProductCategory
                        {
                            Name = "Hair"
                        },
                    new ProductCategory
                        {
                            Name = "Skin"
                        },
                    new ProductCategory
                        {  
                            Name = "Body"
                        }
                };

                context.ProductCategories.AddRange(categories);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedProductsAsync(DatabaseContext context)
        {
            if (!context.Products.Any())
            {
                var categories = context.ProductCategories.ToList();

                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "REVLON Uniq One All in one Super10R Hair mask",
                        Price = 22.00m,
                        Description = "Nourishing oil for smooth and shiny hair.",
                        ProductCategoryId = categories.First(c => c.Name == "Hair").Id,
                        ImagePath = "/images/products/hair/Hair-mask.jpg",
                        Volume = 100
                    },
                    new Product
                    {
                        Name = "VENN Probiotics Cica Complex Biome Booster",
                        Price = 15.50m,
                        Description = "Gentle cleanser with natural citrus extracts.",
                        ProductCategoryId = categories.First(c => c.Name == "Skin").Id,
                        ImagePath = "/images/products/skin/venn-probiotics-cica-complex-biome-booster.png",
                        Volume = 150
                    },
                    new Product
                    {
                        Name = "ELEMIS Muscle reviving body gel",
                        Price = 19.99m,
                        Description = "Soothing lavender-scented lotion for daily hydration.",
                        ProductCategoryId = categories.First(c => c.Name == "Body").Id,
                        ImagePath = "/images/products/body/elemis-skin-nourishing-milk-bath-.png",
                        Volume = 100
                    }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedTreatmentCategoriesAsync(DatabaseContext context)
        {
            if (!context.TreatmentCategories.Any())
            {
                var categories = new List<TreatmentCategory>
                {
                    new TreatmentCategory
                        {
                            CategoryName = "Hair"
                        },
                    new TreatmentCategory
                        {
                            CategoryName = "Skin"
                        },
                    new TreatmentCategory
                        {
                            CategoryName = "Body"
                        }
                };

                context.TreatmentCategories.AddRange(categories);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedTreatmentsAsync(DatabaseContext context)
        {
            if (!context.Treatments.Any())
            {
                var categories = context.TreatmentCategories.ToList();

                var treatments = new List<Treatment>
                {
                    new Treatment
                    {
                        ServiceName = "Hair Spa",
                        Price = 30.00m,
                        Description = "A relaxing hair spa treatment to nourish and revitalize your hair.",
                        TreatmentCategoryId = categories.First(c => c.CategoryName == "Hair").Id
                    },
                    new Treatment
                    {
                        ServiceName = "Facial",
                        Price = 45.00m,
                        Description = "A soothing facial treatment for a fresh and glowing skin.",
                        TreatmentCategoryId = categories.First(c => c.CategoryName == "Skin").Id
                    },
                    new Treatment
                    {
                        ServiceName = "Full Body Massage",
                        Price = 50.00m,
                        Description = "A therapeutic full body massage for relaxation and stress relief.",
                        TreatmentCategoryId = categories.First(c => c.CategoryName == "Body").Id
                    }
                };

                context.Treatments.AddRange(treatments);
                await context.SaveChangesAsync();
            }

        }

        public static async Task SeedReservationsAsync(DatabaseContext context, UserManager<User> userManager)
        {
            if (!context.Reservations.Any())
            {
                var users = userManager.Users.ToList();
                var treatments = context.Treatments.ToList();

                if (users.Any() && treatments.Any())
                {
                    var reservations = new List<Reservation>
                    {
                        new Reservation
                        {
                            UserId = users.First(u => u.UserName == "client").Id,
                            TreatmentId = treatments.First(t => t.ServiceName == "Hair Spa").Id,
                            ReservationDate = DateTime.Now.AddDays(2),
                            Status = "Pending"
                        },
                        new Reservation
                        {
                            UserId = users.First(u => u.UserName == "client").Id,
                            TreatmentId = treatments.First(t => t.ServiceName == "Facial").Id,
                            ReservationDate = DateTime.Now.AddDays(3),
                            Status = "Pending"
                        },
                        new Reservation
                        {
                            UserId = users.First(u => u.UserName == "client").Id,
                            TreatmentId = treatments.First(t => t.ServiceName == "Full Body Massage").Id,
                            ReservationDate = DateTime.Now.AddDays(1),
                            Status = "Confirmed"
                        }
                    };

                    context.Reservations.AddRange(reservations);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
