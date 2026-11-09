using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationsService.Core.Entities;
using NotificationsService.Infrastructure.EntityConfigurations.Base;

namespace NotificationsService.Infrastructure.EntityConfigurations;

public sealed class NotificationConfiguration : BaseEntityConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> entityBuilder)
    {
        base.Configure(entityBuilder);

        entityBuilder.ToTable("notifications_tbl");

        entityBuilder.Property(entity => entity.SourceEventId)
            .IsRequired();

        entityBuilder.Property(entity => entity.Payload)
            .HasColumnType("jsonb")
            .IsRequired();

        entityBuilder.Property(entity => entity.UserId)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.NotificationDeliveries)
            .WithOne(notificationDelivery => notificationDelivery.Notification)
            .HasForeignKey(notificationDelivery => notificationDelivery.NotificationId)
            .IsRequired();
    }
}