using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationsService.Core.Entities;
using NotificationsService.Infrastructure.EntityConfigurations.Base;

namespace NotificationsService.Infrastructure.EntityConfigurations;

public sealed class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> entityBuilder)
    {
        base.Configure(entityBuilder);

        entityBuilder.ToTable("users_tbl");

        entityBuilder.Property(entity => entity.Name)
            .IsRequired();

        entityBuilder.Property(entity => entity.ExternalId)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.Notifications)
            .WithOne(notification => notification.User)
            .HasForeignKey(notification => notification.UserId)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.UserDeliveriesPreferences)
            .WithOne(userDeliveriesPreference => userDeliveriesPreference.User)
            .HasForeignKey(userDeliveriesPreference => userDeliveriesPreference.UserId)
            .IsRequired();

        entityBuilder.HasMany(entity => entity.UserDispatchChannels)
            .WithOne(userDispatchChannel => userDispatchChannel.User)
            .HasForeignKey(userDispatchChannel => userDispatchChannel.UserId)
            .IsRequired();

        entityBuilder.HasIndex(entity => entity.ExternalId)
            .IsUnique();
    }
}