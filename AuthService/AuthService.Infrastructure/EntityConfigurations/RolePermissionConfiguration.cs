using AuthService.Core.Entities;
using AuthService.Infrastructure.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.EntityConfigurations;

public sealed class RolePermissionConfiguration : BaseEntityConfiguration<RolePermissionEntity>
{
    public override void Configure(EntityTypeBuilder<RolePermissionEntity> entity)
    {
        base.Configure(entity);

        entity.ToTable("roles_permissions_tbl");

        entity.Property(entity => entity.RoleId)
            .IsRequired();

        entity.Property(entity => entity.PermissionId)
            .IsRequired();
    }
}