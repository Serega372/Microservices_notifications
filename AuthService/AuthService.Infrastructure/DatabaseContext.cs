using AuthService.Core.Entities;
using AuthService.Core.Entities.Base;
using AuthService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace AuthService.Infrastructure;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options), IUnitOfWork
{
    #region DbSets

    public DbSet<AccessTokenEntity> AccessTokens => Set<AccessTokenEntity>();

    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();

    public DbSet<UserEntity> Users => Set<UserEntity>();

    public DbSet<RoleEntity> Roles => Set<RoleEntity>();

    public DbSet<RolePermissionEntity> RolesPermissions => Set<RolePermissionEntity>();

    public DbSet<PermissionEntity> Permissions => Set<PermissionEntity>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "entity");
                var property = Expression.Property(parameter, nameof(AEntity.IsDeleted));
                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);

                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(lambda);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AEntity entity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreationDate = utcNow;
                        entity.LastUpdate = utcNow;
                        break;

                    case EntityState.Modified:
                        entity.LastUpdate = utcNow;
                        break;

                    case EntityState.Deleted:
                        if (!entity.IsDeleted)
                        {
                            entry.State = EntityState.Modified;
                            entity.IsDeleted = true;
                            entity.DeletedAt = utcNow;
                            entity.LastUpdate = utcNow;
                        }
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) => 
        base.Database.BeginTransactionAsync(cancellationToken);

    public Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default) =>
        transaction.CommitAsync(cancellationToken);

    public Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default) =>
        transaction.RollbackAsync(cancellationToken);
}