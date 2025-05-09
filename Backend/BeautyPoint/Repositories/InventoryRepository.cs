using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;

namespace BeautyPoint.Repositories
{
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(DatabaseContext context) : base(context)
        {
        }
    }
}