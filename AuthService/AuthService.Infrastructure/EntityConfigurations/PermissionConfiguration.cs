using AuthService.Core.Entities;
using AuthService.Infrastructure.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.EntityConfigurations;

public sealed class PermissionConfiguration : BaseEntityConfiguration<PermissionEntity>
{
    public override void Configure(EntityTypeBuilder<PermissionEntity> entity)
    {
        base.Configure(entity);

        entity.ToTable("permissions_tbl");

        entity.HasMany(entity => entity.RolesPermissions)
            .WithOne(rolePermission => rolePermission.Permission)
            .HasForeignKey(rolePermission => rolePermission.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}