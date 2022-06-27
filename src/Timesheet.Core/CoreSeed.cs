using Microsoft.EntityFrameworkCore;
using Timesheet.Core.Entities;
using Timesheet.Core.Interfaces;
using Timesheet.SharedKernel;

namespace Timesheet.Core;

public static class CoreSeed
{
	public static async Task Seeder(IApplicationDbContext dbContext)
	{
		await SeedPermissionAsync(dbContext);

		await SeedRoleAsync(dbContext);

		if (!await dbContext.Users.AnyAsync(e => e.NormalizedUsername == "SA@MAIL.COM"))
		{
			var role = await dbContext.Roles.Where(e => e.NormalizedName == "SUPERADMINISTRATOR").FirstAsync();
			string s = RandomHelper.GetSecureRandomString(64);
			var sa = new User
			{
				Username = "sa@mail.com",
				NormalizedUsername = "SA@MAIL.COM",
				Salt = s,
				HashedPassword = string.Concat(s, "Qwerty@1234").ToSHA512()
			};
			sa.UpdateName("Super", null, "Administrator");
			sa.UserRoles.Add(new UserRole {RoleId = role.RoleId});
			sa.UserPasswords.Add(new UserPassword {Salt = s, HashedPassword = sa.HashedPassword});

			await dbContext.Users.AddAsync(sa);
			await dbContext.SaveChangesAsync(CancellationToken.None);
		}

		await CheckSuperAdministratorRoleHasAllPermissionAsync(dbContext);
	}

	private static async Task CheckSuperAdministratorRoleHasAllPermissionAsync(IApplicationDbContext dbContext)
	{
		var permissions = await dbContext.Permissions.ToListAsync();
		var role = await dbContext.Roles.Include(e => e.RolePermissions).Where(e => e.RoleId == Guid.Empty)
			.FirstAsync();

		bool hasNew = false;
		foreach (var item in permissions)
		{
			if (role.RolePermissions.Any(e => e.PermissionId == item.PermissionId))
				continue;

			hasNew = true;
			role.RolePermissions.Add(new RolePermission {PermissionId = item.PermissionId});
		}

		if (hasNew)
			await dbContext.SaveChangesAsync(CancellationToken.None);
	}

	private static async Task SeedRoleAsync(IApplicationDbContext dbContext)
	{
		if (!await dbContext.Roles.AnyAsync(e => e.RoleId == Guid.Empty))
		{
			var s = "SuperAdministrator";
			dbContext.Roles.Add(new Role {RoleId = Guid.Empty, Name = s, NormalizedName = s.ToUpperInvariant()});
			await dbContext.SaveChangesAsync(CancellationToken.None);
		}
	}

	private static async Task SeedPermissionAsync(IApplicationDbContext dbContext)
	{
		bool hasNewPermission = false;
		if (!await dbContext.Permissions.AnyAsync(e => e.Name == "users.read"))
		{
			hasNewPermission = true;
			dbContext.Permissions.Add(new Permission {Name = "users.read"});
		}

		if (!await dbContext.Permissions.AnyAsync(e => e.Name == "users.readwrite"))
		{
			hasNewPermission = true;
			dbContext.Permissions.Add(new Permission {Name = "users.readwrite"});
		}

		if (hasNewPermission)
			await dbContext.SaveChangesAsync(CancellationToken.None);
	}
}