using System.Linq.Expressions;
using BeautyPoint.Helper;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");
    Task<TEntity> GetByIdAsync(int id, string includeProperties = ""); 
    Task<TEntity> GetByIdAsync(string id, string includeProperties = ""); 
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<PagedList<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter = null,
        int pageNumber = 1, int pageSize = 10, string includeProperties = "");
}