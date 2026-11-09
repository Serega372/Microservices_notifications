using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationsService.Core.Entities;
using NotificationsService.Infrastructure.EntityConfigurations.Base;

namespace NotificationsService.Infrastructure.EntityConfigurations;

public sealed class NotificationTemplateConfiguration : BaseEntityConfiguration<NotificationTemplate>
{
    public override void Configure(EntityTypeBuilder<NotificationTemplate> entityBuilder)
    {
        base.Configure(entityBuilder);

        entityBuilder.ToTable("notification_templates_tbl");

        entityBuilder.Property(entity => entity.Name)
            .IsRequired();

        entityBuilder.Property(entity => entity.Subject);

        entityBuilder.Property(entity => entity.Body)
            .HasColumnType("text")
            .IsRequired();

        entityBuilder.Property(entity => entity.NotificationRuleChannelId)
           .IsRequired();

        entityBuilder.HasMany(entity => entity.NotificationDeliveries)
            .WithOne(notificationDelivery => notificationDelivery.NotificationTemplate)
            .HasForeignKey(notificationDelivery => notificationDelivery.NotificationTemplateId)
            .IsRequired();
    }
}