using BeautyPoint.Data;
using BeautyPoint.Models;
using BeautyPoint.Repositories.Interfaces;

namespace BeautyPoint.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(DatabaseContext context) : base(context)
        {
        }
    }
}