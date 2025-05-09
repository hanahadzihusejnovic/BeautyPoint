using AutoMapper;
using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.SearchObjects;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace BeautyPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IGenericRepository<ProductCategory> _categoryRepository;
        private readonly IMapper _mapper;

        public ProductCategoryController(
            IGenericRepository<ProductCategory> categoryRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProductCategory(ProductCategoryVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var productCategory = _mapper.Map<ProductCategory>(model);

            await _categoryRepository.AddAsync(productCategory);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<ProductCategoryVModel>(productCategory));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetProductCategory(int id)
        {
            var productCategory = await _categoryRepository.GetByIdAsync(id, "Products,Products.ProductReviews,Products.ProductReviews.User");

            if (productCategory == null)
            {
                return NotFound();
            }

            var productCategoryVModel = _mapper.Map<ProductCategoryVModel>(productCategory);
            return Ok(productCategoryVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var productCategoriesQuery = await _categoryRepository.GetAllAsync(includeProperties: "Products,Products.ProductReviews,Products.ProductReviews.User");

            var totalCount = productCategoriesQuery.Count();
            var productCategories = productCategoriesQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var productCategoriesList = _mapper.Map<List<ProductCategoryVModel>>(productCategories);
            return Ok(productCategoriesList);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductCategoryVModel model, CancellationToken cancellationToken = default)
        {
            var productCategory = await _categoryRepository.GetByIdAsync(id);

            if (productCategory == null)
            {
                return NotFound();
            }

            _mapper.Map(model, productCategory);

            await _categoryRepository.UpdateAsync(productCategory);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var productCategory = await _categoryRepository.GetByIdAsync(id);

            if (productCategory == null)
            {
                return NotFound();
            }

            await _categoryRepository.DeleteAsync(productCategory);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
