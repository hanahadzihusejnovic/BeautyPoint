using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;

namespace BeautyPoint.Repositories
{
    public class TreatmentReviewRepository : GenericRepository<TreatmentReview>, ITreatmentReviewRepository
    {
        public TreatmentReviewRepository(DatabaseContext context) : base(context)
        {
        }
    }
}