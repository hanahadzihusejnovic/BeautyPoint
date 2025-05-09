using AutoMapper;
using BeautyPoint.Data;
using BeautyPoint.Dtos;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.SearchObjects;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BeautyPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;
        private readonly UserManager<User> _userManager;

        public UserController(
            IGenericRepository<User> userRepository,
            IMapper mapper,
            DatabaseContext databaseContext,
            UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var user = _mapper.Map<User>(model);

            if (model.Email.EndsWith("@employeeBeautyPoint.com"))
            {
                user.Role = UserRole.Employee;
            }
            else if (model.Email.EndsWith("@adminBeautyPoint.com"))
            {
                user.Role = UserRole.Admin;
            }
            else
            {
                user.Role = UserRole.Client;
            }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleClaim = new Claim("UserRole", user.Role.ToString());
            await _userManager.AddClaimAsync(user, roleClaim);

            var userVModel = _mapper.Map<CreateUserDto>(user);

            return Ok(userVModel);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userRepository.GetByIdAsync(id, "Orders,Orders.Payment,Orders.OrderItems,Orders.OrderItems.Product,Favorites,Favorites.Product,Reservations,Reservations.Treatment,WorkSchedules");

            if (user == null)
            {
                return NotFound();
            }

            var userVModel = _mapper.Map<UserVModel>(user);
            return Ok(userVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var usersQuery = _databaseContext.Users
                .Include(u => u.Orders)
                .ThenInclude(o => o.Payment)
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(u => u.Favorites)
                .ThenInclude(f => f.Product)
                .Include(u => u.Reservations)
                .ThenInclude(r => r.Treatment)
                .Include(u => u.WorkSchedules)
                .AsQueryable();

            if (userRole == "Client")
            {
                usersQuery = usersQuery.Where(u => u.Id.ToString() == userId);
            }

            var totalCount = await usersQuery.CountAsync();

            var users = await usersQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            var usersVModels = _mapper.Map<List<UserVModel>>(users);

            var pagedResponse = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize,
                Users = usersVModels
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(new { totalCount, search.PageSize }));

            return Ok(pagedResponse);
        }

        [HttpGet("all-dropdown")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDropdown([FromQuery] BaseSearchObject search)
        {
            var usersQuery = await _userRepository.GetAllAsync(includeProperties: "Orders,Orders.Payment,Orders.OrderItems,Orders.OrderItems.Product,Favorites,Favorites.Product,Reservations,Reservations.Treatment,WorkSchedules");

            var totalCount = usersQuery.Count();

            var users = usersQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var usersList = _mapper.Map<List<UserVModel>>(users);
            return Ok(usersList);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] UserVModel model, CancellationToken cancellationToken = default)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && user.Id.ToString() != userId)
            {
                return Forbid();
            }

            _mapper.Map(model, user);


            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("check-username")]
        public async Task<IActionResult> CheckUsername([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest();

            var exists = await _databaseContext.Users.AnyAsync(x => x.UserName == username);
            return Ok(exists); 
        }
    }
}