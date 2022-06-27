using Microsoft.EntityFrameworkCore;
using Timesheet.Core.Entities;

namespace Timesheet.Core.Interfaces;

public interface IUserEntity
{
	DbSet<User> Users { get; }
	DbSet<UserLogin> UserLogins { get; }
	DbSet<UserRole> UserRoles { get; }
	DbSet<UserPassword> UserPasswords { get; }
	DbSet<UserToken> UserTokens { get; }
	DbSet<Role> Roles { get; }
	DbSet<Permission> Permissions { get; }
	DbSet<RolePermission> RolePermissions { get; }
}