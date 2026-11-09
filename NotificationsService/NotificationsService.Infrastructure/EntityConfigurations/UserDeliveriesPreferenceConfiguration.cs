using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationsService.Core.Entities;
using NotificationsService.Infrastructure.EntityConfigurations.Base;

namespace NotificationsService.Infrastructure.EntityConfigurations;

public sealed class UserDeliveriesPreferenceConfiguration : BaseEntityConfiguration<UserDeliveriesPreference>
{
    public override void Configure(EntityTypeBuilder<UserDeliveriesPreference> entityBuilder)
    {
        base.Configure(entityBuilder);

        entityBuilder.ToTable("user_deliveries_preferences_tbl");

        entityBuilder.Property(entity => entity.Name)
            .IsRequired();

        entityBuilder.Property(entity => entity.Description);

        entityBuilder.Property(entity => entity.IsEnabled);

        entityBuilder.Property(entity => entity.NotificationCategoryId)
            .IsRequired();

        entityBuilder.Property(entity => entity.DispatchChannelId)
            .IsRequired();

        entityBuilder.Property(entity => entity.UserId)
            .IsRequired();
    }
}