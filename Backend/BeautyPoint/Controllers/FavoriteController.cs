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
    [Authorize (Roles = "Client")]
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly IGenericRepository<Favorite> _favoriteRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;

        public FavoriteController(
            IGenericRepository<Favorite> favoriteRepository,
            IMapper mapper,
            DatabaseContext databaseContext)
        {
            _favoriteRepository = favoriteRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFavorite(FavoriteVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var favorite = _mapper.Map<Favorite>(model);

            
            var product = await _databaseContext.Products
                                           .FirstOrDefaultAsync(p => p.Id == model.ProductId);

            if (product == null)
            {
                return BadRequest("Product does not exist.");
            }
            favorite.Product = product;


            var user = await _databaseContext.Users
                                           .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }
            favorite.User = user;

            
            await _favoriteRepository.AddAsync(favorite);
            await _favoriteRepository.SaveChangesAsync(cancellationToken);

            
            return Ok(_mapper.Map<FavoriteVModel>(favorite));
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFavorite(int id)
        {
            var favorite = await _favoriteRepository.GetByIdAsync(id, "Product,User");

            if (favorite == null)
            {
                return NotFound();
            }

            var favoriteVModel = _mapper.Map<FavoriteVModel>(favorite);
            return Ok(favoriteVModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            
            var favoritesQuery = await _favoriteRepository.GetAllAsync(includeProperties: "Product,User");

            var totalCount = favoritesQuery.Count();

            var favorites = favoritesQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var favoritesList = _mapper.Map<List<FavoriteVModel>>(favorites);
            return Ok(favoritesList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FavoriteVModel model, CancellationToken cancellationToken = default)
        {
            var favorite = await _favoriteRepository.GetByIdAsync(id);

            if (favorite == null)
            {
                return NotFound();
            }

            _mapper.Map(model, favorite);


            var product = await _databaseContext.Products
                                         .FirstOrDefaultAsync(p => p.Id == model.ProductId);

            if (product == null)
            {
                return BadRequest("Product does not exist.");
            }
            favorite.Product = product;

            var user = await _databaseContext.Users
                                           .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }
            favorite.User = user;

            await _favoriteRepository.UpdateAsync(favorite);
            await _favoriteRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var favorite = await _favoriteRepository.GetByIdAsync(id);

            if (favorite == null)
            {
                return NotFound();
            }

            await _favoriteRepository.DeleteAsync(favorite);
            await _favoriteRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}