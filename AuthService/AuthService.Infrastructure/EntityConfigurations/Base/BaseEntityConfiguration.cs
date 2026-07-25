using AuthService.Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.EntityConfigurations.Base;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> 
    where TEntity : AEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .UseIdentityAlwaysColumn();

        builder.Property(entity => entity.Name)
            .IsRequired();

        builder.Property(entity => entity.Description);

        builder.Property(entity => entity.CreationDate)
            .IsRequired();

        builder.Property(entity => entity.LastUpdate)
            .IsRequired();

        builder.Property(entity => entity.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(entity => entity.DeletedAt);

        builder.HasIndex(entity => entity.IsDeleted);
    }
}