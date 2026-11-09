using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationsService.Core.Entities;
using NotificationsService.Infrastructure.EntityConfigurations.Base;

namespace NotificationsService.Infrastructure.EntityConfigurations;

public sealed class NotificationRuleChannelConfiguration : BaseEntityConfiguration<NotificationRuleChannel>
{
    public override void Configure(EntityTypeBuilder<NotificationRuleChannel> entityBuilder)
    {
        base.Configure(entityBuilder);

        entityBuilder.ToTable("notification_rule_channels_tbl");

        entityBuilder.Property(entity => entity.Name)
            .IsRequired();

        entityBuilder.Property(entity => entity.DispatchChannelId)
            .IsRequired();

        entityBuilder.Property(entity => entity.NotificationRuleId)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.NotificationTemplates)
            .WithOne(notificationTemplate => notificationTemplate.NotificationRuleChannel)
            .HasForeignKey(notificationTemplate => notificationTemplate.NotificationRuleChannelId)
            .IsRequired();
    }
}