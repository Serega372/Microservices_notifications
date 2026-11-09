using NotificationsService.Core.Entities.Base;
using System.Linq.Expressions;

namespace NotificationsService.Application.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : AEntity
{
    Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<long> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? predicate = null, bool asNoTracking = true,
        CancellationToken cancellationToken = default);

    TEntity Add(TEntity entity);

    TEntity Update(TEntity entity);

    long Delete(TEntity entity);
}