using AuthService.Core.Entities;
using AuthService.Infrastructure.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.EntityConfigurations;

public sealed class UserConfiguration : BaseEntityConfiguration<UserEntity>
{
    public override void Configure(EntityTypeBuilder<UserEntity> entity)
    {
        base.Configure(entity);

        entity.ToTable("users_tbl");

        entity.Property(entity => entity.Username)
            .IsRequired();

        entity.Property(entity => entity.Email)
            .IsRequired();

        entity.Property(entity => entity.PasswordHash)
            .IsRequired();

        entity.Property(entity => entity.RoleId)
            .IsRequired();

        entity.HasMany(entity => entity.AccessTokens)
            .WithOne(accessToken => accessToken.User)
            .HasForeignKey(accessToken => accessToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(entity => entity.RefreshTokens)
            .WithOne(refreshToken => refreshToken.User)
            .HasForeignKey(refreshToken => refreshToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}