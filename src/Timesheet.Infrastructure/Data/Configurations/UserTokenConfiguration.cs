using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timesheet.Core.Entities;

namespace Timesheet.Infrastructure.Data.Configurations;

public class UserTokenConfiguration : BaseEntityConfiguration<UserToken>
{
	protected override void EntityConfiguration(EntityTypeBuilder<UserToken> builder)
	{
		builder.ToTable($"Trx{nameof(UserToken)}");
		builder.HasKey(e => e.UserTokenId);
		builder.Property(e => e.UserTokenId).ValueGeneratedNever();

		builder.Property(e => e.RefreshToken).HasMaxLength(maxLength: 1024);
		builder.HasIndex(e => e.RefreshToken);
	}
}