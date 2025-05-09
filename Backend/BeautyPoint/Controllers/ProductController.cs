using AutoMapper;
using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.SearchObjects;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using Microsoft.VisualStudio.Services.Users;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BeautyPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;

        public ProductController(
            IGenericRepository<Product> productRepository,
            IMapper mapper,
            DatabaseContext databaseContext)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            var product = _mapper.Map<Product>(model);

            var category = await _databaseContext.ProductCategories
                                       .FirstOrDefaultAsync(c => c.Id == model.ProductCategoryId);

            if (category == null)
            {
                return BadRequest("Product category does not exist.");
            }
            product.ProductCategory = category;

            var categoryFolderName = category.Name.ToLower();
            var uploadsFolderPath = Path.Combine("wwwroot", "images", "products", categoryFolderName);

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            if (model.ProductImage != null && model.ProductImage.Length > 0)
            {
                var fileExtension = Path.GetExtension(model.ProductImage.FileName);
                var baseImageName = model.ProductName.Replace(" ", "-").ToLower();
                var fileName = $"{baseImageName}{fileExtension}";
                var filePath = Path.Combine(uploadsFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProductImage.CopyToAsync(stream);
                }

                product.ImagePath = $"/images/products/{categoryFolderName}/{fileName}";
            }
            else
            {
                return BadRequest("Product image is required.");
            }

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<ProductVModel>(product));
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id, "ProductCategory,ProductReviews,ProductReviews.User");

            if (product == null)
            {
                return NotFound();
            }

            var productVModel = _mapper.Map<ProductVModel>(product);
            return Ok(productVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] ProductSearchObject search)
        {
            var productsQuery = _databaseContext.Products
                .Include(p => p.ProductCategory)      
                .Include(p => p.ProductReviews) 
                .ThenInclude(pr => pr.User)
                .AsQueryable();

            if (search.ProductCategoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.ProductCategoryId == search.ProductCategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search.ProductName))
            {
                string productNameLower = search.ProductName.ToLower();
                productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(productNameLower));
            }

            if (search.Volume.HasValue)
                productsQuery = productsQuery.Where(p => p.Volume == search.Volume.Value);

            var totalCount = await productsQuery.CountAsync();

            var products = await productsQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            var productsVModels = _mapper.Map<List<ProductVModel>>(products);

            var pagedResponse = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize,
                Products = productsVModels 
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(new { totalCount, search.PageSize }));

            return Ok(pagedResponse);
        }

        

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductVModel model, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _mapper.Map(model, product);

            product.Id = id;

            var category = await _databaseContext.ProductCategories
                .FirstOrDefaultAsync(c => c.Id == model.ProductCategoryId);
            if (category == null)
            {
                return BadRequest("Product category does not exist.");
            }
            product.ProductCategory = category;

            if (model.ProductImage != null && model.ProductImage.Length > 0)
            {
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    var oldImagePath = Path.Combine("wwwroot", product.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var categoryFolderName = category.Name.ToLower();
                var uploadsFolderPath = Path.Combine("wwwroot", "images", "products", categoryFolderName);
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                var fileExtension = Path.GetExtension(model.ProductImage.FileName);
                var baseImageName = model.ProductName.Replace(" ", "-").ToLower();
                var fileName = $"{baseImageName}{fileExtension}";
                var filePath = Path.Combine(uploadsFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProductImage.CopyToAsync(stream);
                }

                product.ImagePath = $"/images/products/{categoryFolderName}/{fileName}";
            }
            else
            {
                
                if (string.IsNullOrEmpty(product.ImagePath))
                {
                    product.ImagePath = "/images/default-product-image.jpg"; 
                }
            }

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.DeleteAsync(product);
            await _productRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
