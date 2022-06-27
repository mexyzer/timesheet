using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timesheet.Core.Entities;

namespace Timesheet.Infrastructure.Data.Configurations;

public class UserLoginConfiguration : BaseEntityConfiguration<UserLogin>
{
    protected override void EntityConfiguration(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable($"Htr{nameof(UserLogin)}");
        builder.HasKey(e => e.UserLoginId);
        builder.Property(e => e.UserLoginId).ValueGeneratedNever();

        builder.Property(e => e.UserAgent).HasMaxLength(maxLength: 512);
        builder.Property(e => e.UserAgentDetail).HasMaxLength(maxLength: 512);
        builder.Property(e => e.IpAddress).HasMaxLength(maxLength: 256);
        builder.Property(e => e.Country).HasMaxLength(maxLength: 256);
        builder.Property(e => e.CountryCode).HasMaxLength(maxLength: 256);
        builder.Property(e => e.Region).HasMaxLength(maxLength: 256);
        builder.Property(e => e.City).HasMaxLength(maxLength: 256);
        builder.Property(e => e.TimeZone).HasMaxLength(maxLength: 256);
        builder.Property(e => e.Asn).HasMaxLength(maxLength: 256);
        builder.Property(e => e.AsnOrganization).HasMaxLength(maxLength: 256);
    }
}