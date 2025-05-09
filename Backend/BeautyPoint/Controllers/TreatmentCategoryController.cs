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
    public class TreatmentCategoryController : ControllerBase
    {
        private readonly IGenericRepository<TreatmentCategory> _treatmentCategoryRepository;
        private readonly IMapper _mapper;

        public TreatmentCategoryController(
            IGenericRepository<TreatmentCategory> treatmentCategoryRepository,
            IMapper mapper)
        {
            _treatmentCategoryRepository = treatmentCategoryRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> CreateTreatmentCategory(TreatmentCategoryVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var treatmentCategory = _mapper.Map<TreatmentCategory>(model);

            await _treatmentCategoryRepository.AddAsync(treatmentCategory);
            await _treatmentCategoryRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<TreatmentCategoryVModel>(treatmentCategory));

        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Client,Employee,Admin")]
        public async Task<IActionResult> GetTreatmentCategory(int id)
        {
            var treatmentCategory = await _treatmentCategoryRepository.GetByIdAsync(id, "Treatments");

            if (treatmentCategory == null)
            {
                return NotFound();
            }

            var treatmentCategoryVModel = _mapper.Map<TreatmentCategoryVModel>(treatmentCategory);
            return Ok(treatmentCategoryVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client,Employee,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var treatmentCategoriesQuery = await _treatmentCategoryRepository.GetAllAsync(includeProperties: "Treatments");

            var totalCount = treatmentCategoriesQuery.Count();

            var treatmentCategories = treatmentCategoriesQuery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var treatmentCategoriesList = _mapper.Map<List<TreatmentCategoryVModel>>(treatmentCategories);
            return Ok(treatmentCategoriesList);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] TreatmentCategoryVModel model, CancellationToken cancellationToken = default)
        {
            var treatmentCategory = await _treatmentCategoryRepository.GetByIdAsync(id);

            if (treatmentCategory == null)
            {
                return NotFound();
            }

            _mapper.Map(model, treatmentCategory);

            await _treatmentCategoryRepository.UpdateAsync(treatmentCategory);
            await _treatmentCategoryRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var treatmentCategory = await _treatmentCategoryRepository.GetByIdAsync(id);

            if (treatmentCategory == null)
            {
                return NotFound();
            }

            await _treatmentCategoryRepository.DeleteAsync(treatmentCategory);
            await _treatmentCategoryRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
