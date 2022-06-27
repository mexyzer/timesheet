using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timesheet.Core.Entities;

namespace Timesheet.Infrastructure.Data.Configurations;

public class UserRoleConfiguration : BaseEntityConfiguration<UserRole>
{
    protected override void EntityConfiguration(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable($"Trx{nameof(UserRole)}");
        builder.HasKey(e => e.UserRoleId);
        builder.Property(e => e.UserRoleId).ValueGeneratedNever();
    }
}