using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timesheet.SharedKernel;

namespace Timesheet.Infrastructure.Data.Configurations;

public abstract class BaseEntityConfiguration<TBaseEntity> : IEntityTypeConfiguration<TBaseEntity>
    where TBaseEntity : BaseEntity
{
    public void Configure(EntityTypeBuilder<TBaseEntity> builder)
    {
        builder.Ignore(e => e.Events);

        builder.Property(e => e.CreatedBy).HasMaxLength(maxLength: 256);
        builder.HasIndex(e => e.CreatedBy);
        builder.HasIndex(e => e.CreatedDt);
        builder.Property(e => e.LastModifiedBy).HasMaxLength(maxLength: 256);
        builder.HasIndex(e => e.LastModifiedBy);
        builder.HasIndex(e => e.LastModifiedDt);

        builder.HasQueryFilter(e => e.IsActive && e.DeletedDt == null);

        EntityConfiguration(builder);
    }

    protected abstract void EntityConfiguration(EntityTypeBuilder<TBaseEntity> builder);
}