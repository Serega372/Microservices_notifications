using AuthService.Core.Entities.Base;
using AuthService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AuthService.Infrastructure.Repositories.Base;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : AEntity
{
    protected readonly DbSet<TEntity> DbSet;

    protected readonly ILogger Logger;

    protected BaseRepository(
        DatabaseContext context,
        ILogger logger
        )
    {
        Logger = logger;
        DbSet = context.Set<TEntity>();

        Logger.LogInformation($"{GetType().Name} was initialized");
    }

    public async Task<TEntity?> GetByIdAsync(long id, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        var query = asNoTracking
            ? DbSet.AsNoTracking()
            : DbSet;

        query = query.Where(entity => entity.Id == id);

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<long> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        var query = asNoTracking
            ? DbSet.AsNoTracking()
            : DbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = await DbSet.AddAsync(entity, cancellationToken);

        Logger.LogInformation($"{entity} has been inserted in {GetType().Name}");

        return result.Entity;
    }

    public TEntity Update(TEntity entity)
    {
        var result = DbSet.Update(entity);

        Logger.LogInformation($"{GetType().Name} {entity} has been updated in {GetType().Name}");

        return result.Entity;
    }

    public long Delete(TEntity entity)
    {
        var result = DbSet.Remove(entity);

        Logger.LogInformation($"{GetType().Name} {nameof(entity)} with id {result.Entity.Id} has been deleted");

        return result.Entity.Id;
    }
}