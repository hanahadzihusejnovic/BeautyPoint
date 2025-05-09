using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;

namespace BeautyPoint.Repositories
{
    public class WorkScheduleRepository : GenericRepository<WorkSchedule>, IWorkScheduleRepository
    {
        public WorkScheduleRepository(DatabaseContext context) : base(context)
        {
        }
    }
}