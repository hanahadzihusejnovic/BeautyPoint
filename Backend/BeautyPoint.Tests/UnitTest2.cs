using AutoMapper;
using BeautyPoint.Controllers;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace BeautyPoint.Tests
{
    public class ReservationControllerTest2
    {
        private readonly Mock<IGenericRepository<Reservation>> _reservationRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ReservationController _controller;

        public ReservationControllerTest2()
        {
            _reservationRepoMock = new Mock<IGenericRepository<Reservation>>();
            _mapperMock = new Mock<IMapper>();
            _controller = new ReservationController(_reservationRepoMock.Object, _mapperMock.Object, null);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Client"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetReservation_ShouldReturnNotFound_WhenReservationDoesNotExist()
        {
            int reservationId = 999; 
            _reservationRepoMock.Setup(r => r.GetByIdAsync(reservationId, "Treatment,User"))
                                .ReturnsAsync((Reservation)null);

            var result = await _controller.GetReservation(reservationId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
