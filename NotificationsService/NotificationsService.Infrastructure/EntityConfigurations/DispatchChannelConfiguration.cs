using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationsService.Core.Entities;
using NotificationsService.Infrastructure.EntityConfigurations.Base;

namespace NotificationsService.Infrastructure.EntityConfigurations;

public sealed class DispatchChannelConfiguration : BaseEntityConfiguration<DispatchChannel>
{
    public override void Configure(EntityTypeBuilder<DispatchChannel> entityBuilder)
    {
        base.Configure(entityBuilder);

        entityBuilder.ToTable("dispatch_channels_tbl");

        entityBuilder.Property(entity => entity.Name)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.NotificationRuleChannels)
            .WithOne(notificationRuleChannel => notificationRuleChannel.DispatchChannel)
            .HasForeignKey(notificationRuleChannel => notificationRuleChannel.DispatchChannelId)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.UserDeliveriesPreferences)
            .WithOne(userDeliveriesPreference => userDeliveriesPreference.DispatchChannel)
            .HasForeignKey(userDeliveriesPreference => userDeliveriesPreference.DispatchChannelId)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.UserDispatchChannels)
            .WithOne(userDispatchChannel => userDispatchChannel.DispatchChannel)
            .HasForeignKey(userDispatchChannel => userDispatchChannel.DispatchChannelId)
            .IsRequired();

        entityBuilder.HasIndex(entity => entity.Name)
            .IsUnique();
    }
}