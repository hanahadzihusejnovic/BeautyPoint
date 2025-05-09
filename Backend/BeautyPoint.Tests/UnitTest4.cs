using AutoMapper;
using BeautyPoint.Controllers;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace BeautyPoint.Tests
{
    public class TreatmentControllerGetTest4
    {
        private readonly Mock<IGenericRepository<Treatment>> _treatmentRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TreatmentController _controller;

        public TreatmentControllerGetTest4()
        {
            _treatmentRepoMock = new Mock<IGenericRepository<Treatment>>();
            _mapperMock = new Mock<IMapper>();

            _controller = new TreatmentController(
                _treatmentRepoMock.Object,
                _mapperMock.Object,
                null
            );
        }

        [Fact]
        public async Task GetTreatment_ShouldReturnOk_WhenTreatmentExists()
        {
            var treatmentId = 1;
            var treatment = new Treatment { Id = treatmentId, ServiceName = "Test tretman" };
            var treatmentVModel = new TreatmentVModel { Id = treatmentId, ServiceName = "Test tretman" };

            _treatmentRepoMock.Setup(r => r.GetByIdAsync(treatmentId, It.IsAny<string>()))
                              .ReturnsAsync(treatment);

            _mapperMock.Setup(m => m.Map<TreatmentVModel>(treatment))
                       .Returns(treatmentVModel);

            var result = await _controller.GetTreatment(treatmentId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TreatmentVModel>(okResult.Value);
            Assert.Equal(treatmentId, returnValue.Id);
            Assert.Equal("Test tretman", returnValue.ServiceName);
        }
    }
}
