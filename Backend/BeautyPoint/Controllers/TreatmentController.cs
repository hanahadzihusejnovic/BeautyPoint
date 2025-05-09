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
    public class TreatmentController : ControllerBase
    {
        private readonly IGenericRepository<Treatment> _treatmentRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;

        public TreatmentController(
            IGenericRepository<Treatment> treatmentRepository,
            IMapper mapper,
            DatabaseContext databaseContext)
        {
            _treatmentRepository = treatmentRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> CreateTreatment(TreatmentVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var treatment = _mapper.Map<Treatment>(model);

            var treatmentCategory = await _databaseContext.TreatmentCategories
                                           .FirstOrDefaultAsync(c => c.Id == model.TreatmentCategoryId);

            if (treatmentCategory == null)
            {
                return BadRequest("Treatment category does not exist.");
            }

            treatment.TreatmentCategory = treatmentCategory;

            await _treatmentRepository.AddAsync(treatment);
            await _treatmentRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<TreatmentVModel>(treatment));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Employee,Admin")]
        public async Task<IActionResult> GetTreatment(int id)
        {
            var treatment = await _treatmentRepository.GetByIdAsync(id, "TreatmentCategory,TreatmentReviews,TreatmentReviews.User,ServicePackageTreatments,ServicePackageTreatments.ServicePackage");

            if (treatment == null)
            {
                return NotFound();
            }

            var treatmentVModel = _mapper.Map<TreatmentVModel>(treatment);
            return Ok(treatmentVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client,Employee,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var treatmentsQuery = _databaseContext.Treatments
                .Include(t => t.TreatmentCategory)
                .Include(t => t.TreatmentReviews)
                .ThenInclude(tr => tr.User)
                .Include(t => t.ServicePackageTreatments)
                .ThenInclude(spt => spt.ServicePackage)
                .AsQueryable();

            var totalCount = await treatmentsQuery.CountAsync();

            var treatments = await treatmentsQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            var treatmentsVModels = _mapper.Map<List<TreatmentVModel>>(treatments);

            var pagedResponse = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize,
                Treatments = treatmentsVModels
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(new { totalCount, search.PageSize }));

            return Ok(pagedResponse);
        }

        [HttpGet("all-dropdown")]
        public async Task<IActionResult> GetAllDropdown([FromQuery] BaseSearchObject search)
        {
            var treatmentsQuery = await _treatmentRepository.GetAllAsync(includeProperties: "TreatmentCategory,TreatmentReviews,TreatmentReviews.User,ServicePackageTreatments,ServicePackageTreatments.ServicePackage");


            var totalCount = treatmentsQuery.Count();

            var treatments = treatmentsQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var treatmentsList = _mapper.Map<List<TreatmentVModel>>(treatments);
            return Ok(treatmentsList);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] TreatmentVModel model, CancellationToken cancellationToken = default)
        {
            var treatment = await _treatmentRepository.GetByIdAsync(id);

            if (treatment == null)
            {
                return NotFound();
            }

            _mapper.Map(model, treatment);

            var treatmentCategory = await _databaseContext.TreatmentCategories
                                           .FirstOrDefaultAsync(c => c.Id == model.TreatmentCategoryId);

            if (treatmentCategory == null)
            {
                return BadRequest("Treatment category does not exist.");
            }

            treatment.TreatmentCategory = treatmentCategory;

            await _treatmentRepository.UpdateAsync(treatment);
            await _treatmentRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var treatment = await _treatmentRepository.GetByIdAsync(id);

            if (treatment == null)
            {
                return NotFound();
            }

            await _treatmentRepository.DeleteAsync(treatment);
            await _treatmentRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
