using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationsService.Core.Entities;
using NotificationsService.Infrastructure.EntityConfigurations.Base;

namespace NotificationsService.Infrastructure.EntityConfigurations;

public sealed class UserDispatchChannelConfiguration : BaseEntityConfiguration<UserDispatchChannel>
{
    public override void Configure(EntityTypeBuilder<UserDispatchChannel> entityBuilder)
    {
        base.Configure(entityBuilder);

        entityBuilder.ToTable("user_dispatch_channels_tbl");

        entityBuilder.Property(entity => entity.Name)
            .IsRequired();

        entityBuilder.Property(entity => entity.Description);

        entityBuilder.Property(entity => entity.IsEnabled);

        entityBuilder.Property(entity => entity.IsVerified);

        entityBuilder.Property(entity => entity.DestinationAddress)
            .IsRequired();

        entityBuilder.Property(entity => entity.DispatchChannelId)
            .IsRequired();

        entityBuilder.Property(entity => entity.UserId)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.NotificationDeliveries)
            .WithOne(notificationDelivery => notificationDelivery.UserDispatchChannel)
            .HasForeignKey(notificationDelivery => notificationDelivery.UserDispatchChannelId)
            .IsRequired();
    }
}