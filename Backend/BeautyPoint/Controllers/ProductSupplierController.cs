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
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductSupplierController : ControllerBase
    {
        private readonly IGenericRepository<ProductSupplier> _productSupplierRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;

        public ProductSupplierController(
            IGenericRepository<ProductSupplier> productSupplierRepository,
            IMapper mapper,
            DatabaseContext databaseContext)
        {
            _productSupplierRepository = productSupplierRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductSupplier(ProductSupplierVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var productSupplier = _mapper.Map<ProductSupplier>(model);

            var product = await _databaseContext.Products
                                           .FirstOrDefaultAsync(p => p.Id == model.ProductId);

            if (product == null)
            {
                return BadRequest("Product does not exist.");
            }

            productSupplier.Product = product;

            var supplier = await _databaseContext.Suppliers
                                           .FirstOrDefaultAsync(s => s.Id == model.SupplierId);

            if (supplier == null)
            {
                return BadRequest("Supplier does not exist.");
            }

            productSupplier.Supplier = supplier;

            await _productSupplierRepository.AddAsync(productSupplier);
            await _productSupplierRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<ProductSupplierVModel>(productSupplier));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductSupplier(int id)
        {
            var productSupplier = await _productSupplierRepository.GetByIdAsync(id, "Product,Supplier");

            if (productSupplier == null)
            {
                return NotFound();
            }

            var productSupplierVModel = _mapper.Map<ProductSupplierVModel>(productSupplier);
            return Ok(productSupplierVModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var productSuppliersQuery = await _productSupplierRepository.GetAllAsync(includeProperties: "Product,Supplier");

            var totalCount = productSuppliersQuery.Count();

            var productSuppliers = productSuppliersQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var productSuppliersList = _mapper.Map<List<ProductSupplierVModel>>(productSuppliers);
            return Ok(productSuppliersList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductSupplierVModel model, CancellationToken cancellationToken = default)
        {
            var productSupplier = await _productSupplierRepository.GetByIdAsync(id);

            if (productSupplier == null)
            {
                return NotFound();
            }

            _mapper.Map(model, productSupplier);

            var product = await _databaseContext.Products
                                           .FirstOrDefaultAsync(p => p.Id == model.ProductId);

            if (product == null)
            {
                return BadRequest("Product does not exist.");
            }

            productSupplier.Product = product;

            var supplier = await _databaseContext.Suppliers
                                           .FirstOrDefaultAsync(s => s.Id == model.SupplierId);

            if (supplier == null)
            {
                return BadRequest("Supplier does not exist.");
            }

            productSupplier.Supplier = supplier;

            await _productSupplierRepository.UpdateAsync(productSupplier);
            await _productSupplierRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var productSupplier = await _productSupplierRepository.GetByIdAsync(id);

            if (productSupplier == null)
            {
                return NotFound();
            }

            await _productSupplierRepository.DeleteAsync(productSupplier);
            await _productSupplierRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
