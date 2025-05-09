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
    public class ServicePackageController : ControllerBase
    {
        private readonly IGenericRepository<ServicePackage> _servicePackageRepository;
        private readonly IMapper _mapper;

        public ServicePackageController(
            IGenericRepository<ServicePackage> servicePackageRepository,
            IMapper mapper)
        {
            _servicePackageRepository = servicePackageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> CreateServicePackage(ServicePackageVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var servicePackage = _mapper.Map<ServicePackage>(model);

            await _servicePackageRepository.AddAsync(servicePackage);
            await _servicePackageRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<ServicePackageVModel>(servicePackage));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Employee,Admin")]
        public async Task<IActionResult> GetServicePackage(int id)
        {
            var servicePackage = await _servicePackageRepository.GetByIdAsync(id, "ServicePackageTreatments,ServicePackageTreatments.Treatment");

            if (servicePackage == null)
            {
                return NotFound();
            }

            var servicePackageVModel = _mapper.Map<ServicePackageVModel>(servicePackage);
            return Ok(servicePackageVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client,Employee,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] ServicePackageSearchObject search)
        {
            var servicePackagesQuery = await _servicePackageRepository.GetAllAsync(includeProperties: "ServicePackageTreatments,ServicePackageTreatments.Treatment");

            if (!string.IsNullOrWhiteSpace(search.ServicePackageName))
            {
                servicePackagesQuery = servicePackagesQuery.Where(s => s.PackageName.Contains(search.ServicePackageName));
            }

            if (search.MinServicePackagePrice.HasValue)
            {
                servicePackagesQuery = servicePackagesQuery.Where(s => s.Price >= search.MinServicePackagePrice.Value);
            }

            if (search.MaxServicePackagePrice.HasValue)
            {
                servicePackagesQuery = servicePackagesQuery.Where(s => s.Price <= search.MaxServicePackagePrice.Value);
            }

            var totalCount = servicePackagesQuery.Count();

            var servicePackages = servicePackagesQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var servicePackagesList = _mapper.Map<List<ServicePackageVModel>>(servicePackages);
            return Ok(servicePackagesList);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ServicePackageVModel model, CancellationToken cancellationToken = default)
        {
            var servicePackage = await _servicePackageRepository.GetByIdAsync(id);

            if (servicePackage == null)
            {
                return NotFound();
            }

            _mapper.Map(model, servicePackage);

            await _servicePackageRepository.UpdateAsync(servicePackage);
            await _servicePackageRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var servicePackage = await _servicePackageRepository.GetByIdAsync(id);

            if (servicePackage == null)
            {
                return NotFound();
            }

            await _servicePackageRepository.DeleteAsync(servicePackage);
            await _servicePackageRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
