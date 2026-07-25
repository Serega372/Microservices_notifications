using AuthService.Core.Entities;
using AuthService.Infrastructure.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.EntityConfigurations;

public sealed class RoleConfiguration : BaseEntityConfiguration<RoleEntity>
{
    public override void Configure(EntityTypeBuilder<RoleEntity> entity)
    {
        base.Configure(entity);

        entity.ToTable("roles_tbl");

        entity.HasMany(entity => entity.Users)
            .WithOne(user => user.Role)
            .HasForeignKey(user => user.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(entity => entity.RolesPermissions)
            .WithOne(rolePermission => rolePermission.Role)
            .HasForeignKey(rolePermission => rolePermission.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}