using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NotificationsService.Application.Interfaces;
using NotificationsService.Core.Entities;
using NotificationsService.Core.Entities.Base;
using System.Linq.Expressions;

namespace NotificationsService.Infrastructure;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options), IUnitOfWork
{
    #region DbSets

    public DbSet<DispatchChannel> DispatchChannels => Set<DispatchChannel>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<NotificationCategory> NotificationCategories => Set<NotificationCategory>();

    public DbSet<NotificationDelivery> NotificationDeliveries => Set<NotificationDelivery>();

    public DbSet<NotificationRule> NotificationRules => Set<NotificationRule>();

    public DbSet<NotificationRuleChannel> NotificationRuleChannels => Set<NotificationRuleChannel>();

    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();

    public DbSet<User> Users => Set<User>();

    public DbSet<UserDeliveriesPreference> UserDeliveriesPreferences => Set<UserDeliveriesPreference>();

    public DbSet<UserDispatchChannel> UserDispatchChannels => Set<UserDispatchChannel>();

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

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await Database.BeginTransactionAsync(cancellationToken);
        return new UnitOfWorkTransaction(transaction);
    }
}