using System;
using System.ComponentModel.DataAnnotations;

namespace BeautyPoint.ViewModels
{
    public class ReservationVModel
    {
        [Required(ErrorMessage = "Reservation ID is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "User first name is required.")]
        [StringLength(50, ErrorMessage = "User first name cannot be longer than 50 characters.")]
        public string UserFirstName { get; set; }

        [Required(ErrorMessage = "User last name is required.")]
        [StringLength(50, ErrorMessage = "User last name cannot be longer than 50 characters.")]
        public string UserLastName { get; set; }

        [Required(ErrorMessage = "Service ID is required.")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Service name is required.")]
        [StringLength(100, ErrorMessage = "Service name cannot be longer than 100 characters.")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "Reservation date is required.")]
        public DateTime ReservationDate { get; set; }

        [Required(ErrorMessage = "Reservation status is required.")]
        [StringLength(20, ErrorMessage = "Reservation status cannot be longer than 20 characters.")]
        public string ReservationStatus { get; set; }
    }
}
