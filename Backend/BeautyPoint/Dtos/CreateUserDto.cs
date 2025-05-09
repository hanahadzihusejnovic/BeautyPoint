using BeautyPoint.ViewModels;

namespace BeautyPoint.Dtos
{
    public class CreateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<OrderVModel> Orders { get; set; } = new List<OrderVModel>();
        public ICollection<FavoriteVModel> Favorites { get; set; } = new List<FavoriteVModel>();
        public ICollection<ReservationVModel> Reservations { get; set; } = new List<ReservationVModel>();
        public ICollection<WorkScheduleVModel> WorkSchedules { get; set; } = new List<WorkScheduleVModel>();
    }
}
