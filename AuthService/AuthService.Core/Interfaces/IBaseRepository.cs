using AuthService.Core.Entities.Base;
using System.Linq.Expressions;

namespace AuthService.Core.Interfaces;

public interface IBaseRepository<TEntity>
    where TEntity : AEntity
{
    Task<TEntity?> GetByIdAsync(long id, bool asNoTracking = true, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? predicate = null, bool asNoTracking = true,
        CancellationToken cancellationToken = default);

    Task<long> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    TEntity Update(TEntity entity);

    long Delete(TEntity entity);
}