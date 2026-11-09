using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationsService.Core.Entities;
using NotificationsService.Infrastructure.EntityConfigurations.Base;

namespace NotificationsService.Infrastructure.EntityConfigurations;

public sealed class NotificationCategoryConfiguration : BaseEntityConfiguration<NotificationCategory>
{
    public override void Configure(EntityTypeBuilder<NotificationCategory> entityBuilder)
    {
        base.Configure(entityBuilder);

        entityBuilder.ToTable("notification_categories_tbl");

        entityBuilder.Property(entity => entity.Name)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.NotificationRules)
            .WithOne(notificationRule => notificationRule.NotificationCategory)
            .HasForeignKey(notificationRule => notificationRule.NotificationCategoryId)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.UserDeliveriesPreferences)
            .WithOne(userDeliveriesPreference => userDeliveriesPreference.NotificationCategory)
            .HasForeignKey(userDeliveriesPreference => userDeliveriesPreference.NotificationCategoryId)
            .IsRequired();

        entityBuilder.HasIndex(entity => entity.Name)
            .IsUnique();
    }
}