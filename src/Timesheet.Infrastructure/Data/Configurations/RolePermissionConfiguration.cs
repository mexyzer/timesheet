using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timesheet.Core.Entities;

namespace Timesheet.Infrastructure.Data.Configurations;

public class RolePermissionConfiguration : BaseEntityConfiguration<RolePermission>
{
    protected override void EntityConfiguration(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable($"Trx{nameof(RolePermission)}");
        builder.HasKey(e => e.RolePermissionId);
        builder.Property(e => e.RolePermissionId).ValueGeneratedNever();
    }
}