using AutoMapper;
using BeautyPoint.Controllers;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BeautyPoint.Tests
{
    public class ReservationControllerTest1
    {
        private readonly Mock<IGenericRepository<Reservation>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ReservationController _controller;

        public ReservationControllerTest1()
        {
            _mockRepo = new Mock<IGenericRepository<Reservation>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ReservationController(_mockRepo.Object, _mockMapper.Object, null); 
        }

        [Fact]
        public async Task CreateReservation_ShouldReturnBadRequest_WhenModelIsNull()
        {
            ReservationVModel model = null;

            var result = await _controller.CreateReservation(model);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid data.", badRequestResult.Value);
        }
    }
}
