using AutoMapper;
using BeautyPoint.Controllers;
using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace BeautyPoint.Tests
{
    public class TreatmentControllerTest3
    {
        private readonly DatabaseContext _dbContext;
        private readonly Mock<IGenericRepository<Treatment>> _treatmentRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TreatmentController _controller;

        public TreatmentControllerTest3()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("TreatmentTestDb")
                .Options;
            _dbContext = new DatabaseContext(options);

            _treatmentRepoMock = new Mock<IGenericRepository<Treatment>>();
            _mapperMock = new Mock<IMapper>();

            _controller = new TreatmentController(
                _treatmentRepoMock.Object,
                _mapperMock.Object,
                _dbContext
            );
        }

        [Fact]
        public async Task CreateTreatment_ShouldReturnBadRequest_WhenCategoryDoesNotExist()
        {
            var model = new TreatmentVModel
            {
                ServiceName = "Test tretman",
                TreatmentCategoryId = 999 
            };

            _mapperMock.Setup(m => m.Map<Treatment>(It.IsAny<TreatmentVModel>()))
                       .Returns(new Treatment());

            var result = await _controller.CreateTreatment(model);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Treatment category does not exist.", badRequest.Value);
        }
    }
}
