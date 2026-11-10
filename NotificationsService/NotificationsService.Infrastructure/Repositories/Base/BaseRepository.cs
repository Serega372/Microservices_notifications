using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationsService.Application.Interfaces;
using NotificationsService.Core.Entities.Base;
using System.Linq.Expressions;

namespace NotificationsService.Infrastructure.Repositories.Base;

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

        return await query.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
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

    // TODO: Add summary
    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? predicate = null, bool isPaginatedQuery = true, bool asNoTracking = true,
        ushort limit = 0, uint offset = 0, CancellationToken cancellationToken = default)
    {
        if (limit == 0 && isPaginatedQuery)
        {
            Logger.LogCritical(
                "No limit is set for the paginated request in {Path}. To retrieve all items, an explicit flag [isPaginatedQuery] must be overriden",
                GetType().Name);

            // TODO: Add custom exception
            throw new Exception("Exception occured while get entities: No limit is set for the paginated request");
        }

        var query = asNoTracking
            ? DbSet.AsNoTracking()
            : DbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        query = query.OrderBy(entity => entity.Id);
        if (limit != 0)
        {
            query = query.Skip(Convert.ToInt32(offset)).Take(limit);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public TEntity Add(TEntity entity)
    {
        var result = DbSet.Add(entity);
        return result.Entity;
    }

    public TEntity Update(TEntity entity)
    {
        var result = DbSet.Update(entity);
        return result.Entity;
    }

    public long Delete(TEntity entity)
    {
        var result = DbSet.Remove(entity);
        return result.Entity.Id;
    }
}