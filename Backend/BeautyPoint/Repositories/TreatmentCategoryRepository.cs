using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;

namespace BeautyPoint.Repositories
{
    public class TreatmentCategoryRepository : GenericRepository<TreatmentCategory>, ITreatmentCategoryRepository
    {
        public TreatmentCategoryRepository(DatabaseContext context) : base(context)
        {
        }
    }
}