using BeautyPoint.Models;
using BeautyPoint.ViewModels;
using AutoMapper;
using BeautyPoint.ViewModels;
using BeautyPoint.Dtos;

namespace BeautyPoint.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Favorite, FavoriteVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.User.LastName)).ReverseMap();

            CreateMap<Inventory, InventoryVModel>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.QuantityInStock, opt => opt.MapFrom(src => src.QuantityInStock)).ReverseMap();

            CreateMap<OrderItem, OrderItemVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price)).ReverseMap();

            CreateMap<Order, OrderVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.User.City))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Payment.Id))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Payment.PaymentStatus))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Payment.Amount))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Payment.PaymentMethod))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.Payment.TransactionId))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems)).ReverseMap();

            CreateMap<Payment, PaymentVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId)).ReverseMap();

            CreateMap<ProductReview, ProductReviewVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductRating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.ProductComment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.ReviewDate, opt => opt.MapFrom(src => src.ReviewDate)).ReverseMap();


           CreateMap<Product, ProductVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ProductCategoryId, opt => opt.MapFrom(src => src.ProductCategory.Id))
                .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductCategory.Name))
                .ForMember(dest => dest.ProductReviews, opt => opt.MapFrom(src => src.ProductReviews))
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.ProductImage, opt => opt.Ignore()) // ⬅️ Premješteno prije .ReverseMap()
                .ReverseMap();

            CreateMap<ProductCategory, ProductCategoryVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products)).ReverseMap();

            CreateMap<ProductSupplier, ProductSupplierVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.Supplier.Id))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.ContactName)).ReverseMap();

            CreateMap<Reservation, ReservationVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.Treatment.Id))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Treatment.ServiceName))
                .ForMember(dest => dest.ReservationDate, opt => opt.MapFrom(src => src.ReservationDate))
                .ForMember(dest => dest.ReservationStatus, opt => opt.MapFrom(src => src.Status)).ReverseMap();

            CreateMap<ServicePackageTreatment, ServicePackageTreatmentVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ServicePackageId, opt => opt.MapFrom(src => src.ServicePackage.Id))
                .ForMember(dest => dest.ServicePackageName, opt => opt.MapFrom(src => src.ServicePackage.PackageName))
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.Treatment.Id))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Treatment.ServiceName)).ReverseMap();

            CreateMap<ServicePackage, ServicePackageVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.PackageName))
                .ForMember(dest => dest.PackageDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.PackagePrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.ServicePackageTreatments, opt => opt.MapFrom(src => src.ServicePackageTreatments)).ReverseMap();

            CreateMap<Supplier, SupplierVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.ContactName, opt => opt.MapFrom(src => src.ContactName))
                .ForMember(dest => dest.ContactEmail, opt => opt.MapFrom(src => src.ContactEmail))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone)).ReverseMap();

            CreateMap<TreatmentReview, TreatmentReviewVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.Treatment.Id))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Treatment.ServiceName))
                .ForMember(dest => dest.TreatmentRating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.TreatmentComment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.ReviewDate, opt => opt.MapFrom(src => src.ReviewDate)).ReverseMap();

            CreateMap<Treatment, TreatmentVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.ServiceName))
                .ForMember(dest => dest.TreatmentPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.TreatmentDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.TreatmentCategoryId, opt => opt.MapFrom(src => src.TreatmentCategory.Id))
                .ForMember(dest => dest.TreatmentCategoryName, opt => opt.MapFrom(src => src.TreatmentCategory.CategoryName))
                .ForMember(dest => dest.TreatmentReviews, opt => opt.MapFrom(src => src.TreatmentReviews))
                .ForMember(dest => dest.ServicePackageTreatments, opt => opt.MapFrom(src => src.ServicePackageTreatments)).ReverseMap();

            CreateMap<TreatmentCategory, TreatmentCategoryVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Treatments, opt => opt.MapFrom(src => src.Treatments)).ReverseMap();

            CreateMap<User, UserVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders))
                .ForMember(dest => dest.Favorites, opt => opt.MapFrom(src => src.Favorites))
                .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Reservations))
                .ForMember(dest => dest.WorkSchedules, opt => opt.MapFrom(src => src.WorkSchedules)).ReverseMap();

            CreateMap<WorkSchedule, WorkScheduleVModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Employee.Id))
                .ForMember(dest => dest.EmployeeFirstName, opt => opt.MapFrom(src => src.Employee.FirstName))
                .ForMember(dest => dest.EmployeeLastName, opt => opt.MapFrom(src => src.Employee.LastName))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime)).ReverseMap();

            CreateMap<User, CreateUserDto>()
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
               .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash))
               .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders))
               .ForMember(dest => dest.Favorites, opt => opt.MapFrom(src => src.Favorites))
               .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Reservations))
               .ForMember(dest => dest.WorkSchedules, opt => opt.MapFrom(src => src.WorkSchedules)).ReverseMap();
        }
    }
}
