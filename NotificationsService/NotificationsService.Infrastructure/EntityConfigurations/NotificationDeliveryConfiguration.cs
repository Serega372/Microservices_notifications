using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationsService.Core.Entities;
using NotificationsService.Infrastructure.EntityConfigurations.Base;

namespace NotificationsService.Infrastructure.EntityConfigurations;

public sealed class NotificationDeliveryConfiguration : BaseEntityConfiguration<NotificationDelivery>
{
    public override void Configure(EntityTypeBuilder<NotificationDelivery> entityBuilder)
    {
        base.Configure(entityBuilder);

        entityBuilder.ToTable("notification_deliveries_tbl");

        entityBuilder.Property(entity => entity.Subject);

        entityBuilder.Property(entity => entity.Body)
            .HasColumnType("text")
            .IsRequired();

        entityBuilder.Property(entity => entity.LastError)
            .HasColumnType("text");

        entityBuilder.Property(entity => entity.Status)
            .IsRequired();

        entityBuilder.Property(entity => entity.DeliveredAt);

        entityBuilder.Property(entity => entity.AttemptCount)
            .IsRequired();

        entityBuilder.Property(entity => entity.Description);

        entityBuilder.Property(entity => entity.UserDispatchChannelId)
            .IsRequired();

        entityBuilder.Property(entity => entity.NotificationTemplateId)
            .IsRequired();

        entityBuilder.Property(entity => entity.NotificationId)
            .IsRequired();
    }
}