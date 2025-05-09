using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;

namespace BeautyPoint.Repositories
{
    public class ServicePackageTreatmentRepository : GenericRepository<ServicePackageTreatment>, IServicePackageTreatmentRepository
    {
        public ServicePackageTreatmentRepository(DatabaseContext context) : base(context)
        {
        }
    }
}