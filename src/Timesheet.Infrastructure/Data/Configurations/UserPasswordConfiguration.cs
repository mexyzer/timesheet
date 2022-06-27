using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timesheet.Core.Entities;

namespace Timesheet.Infrastructure.Data.Configurations;

public class UserPasswordConfiguration : BaseEntityConfiguration<UserPassword>
{
    protected override void EntityConfiguration(EntityTypeBuilder<UserPassword> builder)
    {
        builder.ToTable($"Htr{nameof(UserPassword)}");
        builder.HasKey(e => e.UserPasswordId);
        builder.Property(e => e.UserPasswordId).ValueGeneratedNever();

        builder.Property(e => e.Salt).HasMaxLength(maxLength: 1024);
        builder.Property(e => e.HashedPassword).HasMaxLength(maxLength: 1024);
    }
}