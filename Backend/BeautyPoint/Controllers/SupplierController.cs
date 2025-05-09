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
    [Authorize(Roles = "Employee,Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly IGenericRepository<Supplier> _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierController(
            IGenericRepository<Supplier> supplierRepository,
            IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplier(SupplierVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var supplier = _mapper.Map<Supplier>(model);

            await _supplierRepository.AddAsync(supplier);
            await _supplierRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<SupplierVModel>(supplier));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplier(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id, "");

            if (supplier == null)
            {
                return NotFound();
            }

            var supplierVModel = _mapper.Map<SupplierVModel>(supplier);
            return Ok(supplierVModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var suppliersQuery = await _supplierRepository.GetAllAsync(includeProperties: "");

            var totalCount = suppliersQuery.Count();

            var suppliers = suppliersQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var suppliersList = _mapper.Map<List<SupplierVModel>>(suppliers);
            return Ok(suppliersList);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SupplierVModel model, CancellationToken cancellationToken = default)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            _mapper.Map(model, supplier);

            await _supplierRepository.UpdateAsync(supplier);
            await _supplierRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            await _supplierRepository.DeleteAsync(supplier);
            await _supplierRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
