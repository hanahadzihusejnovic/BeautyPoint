using AutoMapper;
using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.SearchObjects;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BeautyPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicePackageTreatmentController : ControllerBase
    {
        private readonly IGenericRepository<ServicePackageTreatment> _servicePackageTreatmentRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;

        public ServicePackageTreatmentController(
            IGenericRepository<ServicePackageTreatment> servicePackageTreatmentRepository,
            IMapper mapper,
            DatabaseContext databaseContext)
        {
            _servicePackageTreatmentRepository = servicePackageTreatmentRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> CreateServicePackageTreatment(ServicePackageTreatmentVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var servicePackageTreatment = _mapper.Map<ServicePackageTreatment>(model);

            var servicePackage = await _databaseContext.ServicePackages
                                           .FirstOrDefaultAsync(s => s.Id == model.ServicePackageId);

            if (servicePackage == null)
            {
                return BadRequest("Service package does not exist.");
            }

            servicePackageTreatment.ServicePackage = servicePackage;

            var treatment = await _databaseContext.Treatments
                                           .FirstOrDefaultAsync(t => t.Id == model.ServiceId);

            if (treatment == null)
            {
                return BadRequest("Treatment does not exist.");
            }

            servicePackageTreatment.Treatment = treatment;

            await _servicePackageTreatmentRepository.AddAsync(servicePackageTreatment);
            await _servicePackageTreatmentRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<ServicePackageTreatmentVModel>(servicePackageTreatment));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Employee,Admin")]
        public async Task<IActionResult> GetServicePackageTreatment(int id)
        {
            var servicePackageTreatment = await _servicePackageTreatmentRepository.GetByIdAsync(id, "ServicePackage,Treatment");

            if (servicePackageTreatment == null)
            {
                return NotFound();
            }

            var servicePackageTreatmentVModel = _mapper.Map<ServicePackageTreatmentVModel>(servicePackageTreatment);
            return Ok(servicePackageTreatmentVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client,Employee,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var servicePackageTreatmentsQuery = await _servicePackageTreatmentRepository.GetAllAsync(includeProperties: "ServicePackage,Treatment");

            var totalCount = servicePackageTreatmentsQuery.Count();

            var servicePackageTreatments = servicePackageTreatmentsQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var servicePackageTreatmentsList = _mapper.Map<List<ServicePackageTreatmentVModel>>(servicePackageTreatments);
            return Ok(servicePackageTreatmentsList);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ServicePackageTreatmentVModel model, CancellationToken cancellationToken = default)
        {
            var servicePackageTreatment = await _servicePackageTreatmentRepository.GetByIdAsync(id);

            if (servicePackageTreatment == null)
            {
                return NotFound();
            }

            _mapper.Map(model, servicePackageTreatment);

            var servicePackage = await _databaseContext.ServicePackages
                                           .FirstOrDefaultAsync(s => s.Id == model.ServicePackageId);

            if (servicePackage == null)
            {
                return BadRequest("Service package does not exist.");
            }

            servicePackageTreatment.ServicePackage = servicePackage;

            var treatment = await _databaseContext.Treatments
                                           .FirstOrDefaultAsync(t => t.Id == model.ServiceId);

            if (treatment == null)
            {
                return BadRequest("Treatment does not exist.");
            }

            servicePackageTreatment.Treatment = treatment;

            await _servicePackageTreatmentRepository.UpdateAsync(servicePackageTreatment);
            await _servicePackageTreatmentRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var servicePackageTreatment = await _servicePackageTreatmentRepository.GetByIdAsync(id);

            if (servicePackageTreatment == null)
            {
                return NotFound();
            }

            await _servicePackageTreatmentRepository.DeleteAsync(servicePackageTreatment);
            await _servicePackageTreatmentRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
