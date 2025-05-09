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
    public class WorkScheduleController : ControllerBase
    {
        private readonly IGenericRepository<WorkSchedule> _workScheduleRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseContext _databaseContext;

        public WorkScheduleController(
            IGenericRepository<WorkSchedule> workScheduleRepository,
            IMapper mapper,
            DatabaseContext databaseContext)
        {
            _workScheduleRepository = workScheduleRepository;
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        [HttpPost]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> CreateWorkSchedule(WorkScheduleVModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var workSchedule = _mapper.Map<WorkSchedule>(model);

            var employee = await _databaseContext.Users
                                        .FirstOrDefaultAsync(u => u.Id == model.EmployeeId && u.Role == UserRole.Employee);

            if (employee == null)
            {
                return BadRequest("User does not exist or is not an employee.");
            }
            workSchedule.Employee = employee;

            await _workScheduleRepository.AddAsync(workSchedule);
            await _workScheduleRepository.SaveChangesAsync(cancellationToken);

            return Ok(_mapper.Map<WorkScheduleVModel>(workSchedule));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> GetWorkSchedule(int id)
        {
            var workSchedule = await _workScheduleRepository.GetByIdAsync(id, "Employee");

            if (workSchedule == null)
            {
                return NotFound();
            }

            var workScheduleVModel = _mapper.Map<WorkScheduleVModel>(workSchedule);
            return Ok(workScheduleVModel);
        }

        [HttpGet]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> GetAll([FromQuery] BaseSearchObject search)
        {
            var workSchedulesquery = await _workScheduleRepository.GetAllAsync(includeProperties: "Employee");

            var totalCount = workSchedulesquery.Count();

            var workSchedules = workSchedulesquery
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            var metaData = new
            {
                TotalCount = totalCount,
                PageSize = search.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metaData));

            var workSchedulesList = _mapper.Map<List<WorkScheduleVModel>>(workSchedules);
            return Ok(workSchedulesList);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] WorkScheduleVModel model, CancellationToken cancellationToken = default)
        {
            var workSchedule = await _workScheduleRepository.GetByIdAsync(id);

            if (workSchedule == null)
            {
                return NotFound();
            }

            _mapper.Map(model, workSchedule);

            var employee = await _databaseContext.Users
                                        .FirstOrDefaultAsync(u => u.Id == model.EmployeeId && u.Role == UserRole.Employee);

            if (employee == null)
            {
                return BadRequest("User does not exist or is not an employee.");
            }
            workSchedule.Employee = employee;

            await _workScheduleRepository.UpdateAsync(workSchedule);
            await _workScheduleRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var workSchedule = await _workScheduleRepository.GetByIdAsync(id);

            if (workSchedule == null)
            {
                return NotFound();
            }

            await _workScheduleRepository.DeleteAsync(workSchedule);
            await _workScheduleRepository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}