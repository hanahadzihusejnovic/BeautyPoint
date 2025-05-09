using AutoMapper;
using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories;
using BeautyPoint.Repositories.Interfaces;
using BeautyPoint.SearchObjects;
using BeautyPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BeautyPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IGenericRepository<Reservation> _reservationRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;

        public ReservationController(
            IGenericRepository<Reservation> reservationRepository,
            IMapper mapper,
            DatabaseContext databaseContext)
        {
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        [HttpPost]
        [Authorize (Roles = "Client,Admin")]
        public async Task<IActionResult> CreateReservation(ReservationVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var reservation = _mapper.Map<Reservation>(model);

            var treatment = await _databaseContext.Treatments
                                           .FirstOrDefaultAsync(t => t.Id == model.ServiceId);

            if (treatment == null)
            {
                return BadRequest("Treatment does not exist.");
            }
            reservation.Treatment = treatment;

            var user = await _databaseContext.Users
                                           .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }

            reservation.User = user;

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<ReservationVModel>(reservation));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Employee,Admin")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var reservation = await _reservationRepository.GetByIdAsync(id, "Treatment,User");

            if (reservation == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && reservation.UserId.ToString() != userId)
            {
                return Forbid();
            }

            var reservationVModel = _mapper.Map<ReservationVModel>(reservation);
            return Ok(reservationVModel);
        }


        [HttpGet]
        [Authorize(Roles = "Client,Employee,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var reservationsQuery = _databaseContext.Reservations
                .Include(r => r.Treatment)
                .Include(r => r.User)
                .AsQueryable();

            if (userRole == "Client")
            {
                reservationsQuery = reservationsQuery.Where(r => r.UserId.ToString() == userId);
            }

            var totalCount = await reservationsQuery.CountAsync();

            var reservations = await reservationsQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            var reservationsVModels = _mapper.Map<List<ReservationVModel>>(reservations);

            var pagedResponse = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize,
                Reservations = reservationsVModels
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(new { totalCount, search.PageSize }));

            return Ok(pagedResponse);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ReservationVModel model, CancellationToken cancellationToken = default)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            _mapper.Map(model, reservation);

            var treatment = await _databaseContext.Treatments
                                          .FirstOrDefaultAsync(t => t.Id == model.ServiceId);

            if (treatment == null)
            {
                return BadRequest("Treatment does not exist.");
            }
            reservation.Treatment = treatment;

            var user = await _databaseContext.Users
                                           .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }
            reservation.User = user;

            await _reservationRepository.UpdateAsync(reservation);
            await _reservationRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Client,Employee,Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var reservation = await _reservationRepository.GetByIdAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && reservation.UserId.ToString() != userId)
            {
                return Forbid();
            }

            await _reservationRepository.DeleteAsync(reservation);
            await _reservationRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}