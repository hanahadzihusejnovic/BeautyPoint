using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;

namespace BeautyPoint.Repositories
{
    public class ServicePackageRepository : GenericRepository<ServicePackage>, IServicePackageRepository
    {
        public ServicePackageRepository(DatabaseContext context) : base(context)
        {
        }
    }
}