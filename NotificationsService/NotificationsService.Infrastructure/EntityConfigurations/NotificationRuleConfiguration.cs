using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationsService.Core.Entities;
using NotificationsService.Infrastructure.EntityConfigurations.Base;

namespace NotificationsService.Infrastructure.EntityConfigurations;

public sealed class NotificationRuleConfiguration : BaseEntityConfiguration<NotificationRule>
{
    public override void Configure(EntityTypeBuilder<NotificationRule> entityBuilder)
    {
        base.Configure(entityBuilder);

        entityBuilder.ToTable("notification_rules_tbl");

        entityBuilder.Property(entity => entity.Name)
            .IsRequired();

        entityBuilder.Property(entity => entity.EventType)
            .IsRequired();

        entityBuilder.Property(entity => entity.NotificationCategoryId)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.NotificationRuleChannels)
            .WithOne(notificationRuleChannel => notificationRuleChannel.NotificationRule)
            .HasForeignKey(notificationRuleChannel => notificationRuleChannel.NotificationRuleId)
            .IsRequired();

        entityBuilder.HasIndex(entity => entity.EventType)
            .IsUnique();
    }
}