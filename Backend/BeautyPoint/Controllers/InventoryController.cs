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
    public class InventoryController : ControllerBase
    {
        private readonly IGenericRepository<Inventory> _inventoryRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;

        public InventoryController(
            IGenericRepository<Inventory> inventoryRepository,
            IMapper mapper,
            DatabaseContext databaseContext)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        [HttpPost]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> CreateInventory(InventoryVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var inventory = _mapper.Map<Inventory>(model);

            
            var product = await _databaseContext.Products
                                           .FirstOrDefaultAsync(p => p.Id == model.ProductId);

            if (product == null)
            {
                return BadRequest("Product does not exist.");
            }
            inventory.Product = product;

           
            await _inventoryRepository.AddAsync(inventory);
            await _inventoryRepository.SaveChangesAsync(cancellationToken);

            
            return Ok(_mapper.Map<InventoryVModel>(inventory));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> GetInventory(int id)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id, "Product");

            if (inventory == null)
            {
                return NotFound();
            }

            var inventoryVModel = _mapper.Map<InventoryVModel>(inventory);
            return Ok(inventoryVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var inventoriesQuery = await _inventoryRepository.GetAllAsync(includeProperties: "Product");

            var totalCount = inventoriesQuery.Count();

            var inventories = inventoriesQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var inventoriesList = _mapper.Map<List<InventoryVModel>>(inventories);
            return Ok(inventoriesList);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] InventoryVModel model, CancellationToken cancellationToken = default)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            _mapper.Map(model, inventory);

            var product = await _databaseContext.Products
                                         .FirstOrDefaultAsync(p => p.Id == model.ProductId);

            if (product == null)
            {
                return BadRequest("Product does not exist.");
            }
            inventory.Product = product;


            await _inventoryRepository.UpdateAsync(inventory);
            await _inventoryRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            await _inventoryRepository.DeleteAsync(inventory);
            await _inventoryRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}