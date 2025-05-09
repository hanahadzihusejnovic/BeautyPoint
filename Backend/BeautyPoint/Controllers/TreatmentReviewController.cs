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
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BeautyPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreatmentReviewController : ControllerBase
    {
        private readonly IGenericRepository<TreatmentReview> _treatmentReviewRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;

        public TreatmentReviewController(
            IGenericRepository<TreatmentReview> treatmentReviewRepository,
            IMapper mapper,
            DatabaseContext databaseContext)
        {
            _treatmentReviewRepository = treatmentReviewRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CreateTreatmentReview(TreatmentReviewVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var treatmentReview = _mapper.Map<TreatmentReview>(model);

            var user = await _databaseContext.Users
                                          .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            treatmentReview.User = user;

            var treatment = await _databaseContext.Treatments
                                          .FirstOrDefaultAsync(t => t.Id == model.ServiceId);

            if (treatment == null)
            {
                return BadRequest("Treatment does not exist.");
            }

            treatmentReview.Treatment = treatment;

            await _treatmentReviewRepository.AddAsync(treatmentReview);
            await _treatmentReviewRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<TreatmentReviewVModel>(treatmentReview));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetTreatmentReview(int id)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var treatmentReview = await _treatmentReviewRepository.GetByIdAsync(id, "User,Treatment");

            if (treatmentReview == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && treatmentReview.UserId.ToString() != userId)
            {
                return Forbid();
            }

            var treatmentReviewVModel = _mapper.Map<TreatmentReviewVModel>(treatmentReview);
            return Ok(treatmentReviewVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var treatmentReviewsQuery = await _treatmentReviewRepository.GetAllAsync(includeProperties: "User,Treatment");

            if (userRole == "Client")
            {
                treatmentReviewsQuery = treatmentReviewsQuery.Where(o => o.UserId.ToString() == userId);
            }

            var totalCount = treatmentReviewsQuery.Count();

            var treatmentReviews = treatmentReviewsQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var treatmentReviewsList = _mapper.Map<List<TreatmentReviewVModel>>(treatmentReviews);
            return Ok(treatmentReviewsList);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] TreatmentReviewVModel model, CancellationToken cancellationToken = default)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var treatmentReview = await _treatmentReviewRepository.GetByIdAsync(id);

            if (treatmentReview == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && treatmentReview.UserId.ToString() != userId)
            {
                return Forbid();
            }

            _mapper.Map(model, treatmentReview);

            var user = await _databaseContext.Users
                                          .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            treatmentReview.User = user;

            var treatment = await _databaseContext.Treatments
                                          .FirstOrDefaultAsync(t => t.Id == model.ServiceId);

            if (treatment == null)
            {
                return BadRequest("Treatment does not exist.");
            }

            treatmentReview.Treatment = treatment;

            await _treatmentReviewRepository.UpdateAsync(treatmentReview);
            await _treatmentReviewRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var treatmentReview = await _treatmentReviewRepository.GetByIdAsync(id);

            if (treatmentReview == null)
            {
                return NotFound();
            }

            if (userRole == "Client" && treatmentReview.UserId.ToString() != userId)
            {
                return Forbid();
            }

            await _treatmentReviewRepository.DeleteAsync(treatmentReview);
            await _treatmentReviewRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}