using Microsoft.EntityFrameworkCore;
using Timesheet.Core.Entities;
using Timesheet.Core.Interfaces;
using Timesheet.SharedKernel;

namespace Timesheet.Infrastructure.Services;

public class DefaultUserService : IUserService
{
	private readonly IApplicationDbContext _dbContext;

	public DefaultUserService(IApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public IQueryable<User> GetBaseUserQueryable()
	{
		var query = _dbContext.Users.AsQueryable();

		query = query.AsSplitQuery()
			.Include(e => e.UserPasswords)
			.Include(e => e.UserRoles)
			.ThenInclude(e => e.Role)
			.ThenInclude(e => e!.RolePermissions)
			.ThenInclude(e => e.Permission);

		return query;
	}

	public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
	{
		var s = username.ToUpperInvariant();

		return GetBaseUserQueryable().Where(e => e.NormalizedUsername == s).FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
	{
		var user = await _dbContext.Users.Where(e => e.UserId == userId).FirstOrDefaultAsync(cancellationToken);
		if (user == null || string.IsNullOrWhiteSpace(user.HashedPassword))
			return false;

		return string.Concat(user.Salt, password).ToSHA512() == user.HashedPassword;
	}

	public Task<bool> IsEmailAddressExists(string email, CancellationToken cancellationToken)
	{
		var emailUpperCase = email.ToUpperInvariant();

		return _dbContext.Users.AnyAsync(e => e.NormalizedUsername == emailUpperCase, cancellationToken);
	}

	public async Task<IReadOnlyList<Role>> GetAllRoleAsync(int page, int size, CancellationToken cancellationToken)
	{
		var results = await _dbContext.Roles.Skip((page - 1) == 0 || (page - 1) < 0 ? 0 : (page - 1) * size).Take(size)
			.ToListAsync(cancellationToken);

		return results;
	}

	public Task<Role?> GetRoleByIdAsync(string id, CancellationToken cancellationToken)
	{
		var gId = new Guid(id);
		return _dbContext.Roles.Include(e => e.RolePermissions)
			.Where(e => e.RoleId == gId).FirstOrDefaultAsync(cancellationToken);
	}

	public Task<bool> IsRoleExistsByNameAsync(string name, CancellationToken cancellationToken)
	{
		string s = name.ToUpperInvariant();

		return _dbContext.Roles.AnyAsync(e => e.NormalizedName == s, cancellationToken);
	}

	public async Task<IReadOnlyList<Permission>> GetAllPermissionAsync(int page, int size,
		CancellationToken cancellationToken)
	{
		var results = await _dbContext.Permissions.Skip((page - 1) == 0 || (page - 1) < 0 ? 0 : (page - 1) * size)
			.Take(size)
			.ToListAsync(cancellationToken);

		return results;
	}

	public Task<Permission?> GetPermissionByIdAsync(string id, CancellationToken cancellationToken)
	{
		var gId = new Guid(id);

		return _dbContext.Permissions.Where(e => e.PermissionId == gId).FirstOrDefaultAsync(cancellationToken);
	}

	public Task<bool> IsPermissionExistsByIdAsync(string id, CancellationToken cancellationToken)
	{
		var gId = new Guid(id);

		return _dbContext.Permissions.AnyAsync(e => e.PermissionId == gId, cancellationToken);
	}

	public Task<Permission?> GetPermissionByNameAsync(string name, CancellationToken cancellationToken)
	{
		name = name.ToLower();
		return _dbContext.Permissions.Where(e => e.Name == name).FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<User>> GetAllUserAsync(int page, int size, CancellationToken cancellationToken)
	{
		var results = await _dbContext.Users.Skip((page - 1) == 0 || (page - 1) < 0 ? 0 : (page - 1) * size)
			.Take(size)
			.ToListAsync(cancellationToken);

		return results;
	}

	public Task<User?> GetUserByIdAsync(string id, CancellationToken cancellationToken)
	{
		var gId = new Guid(id);

		return GetBaseUserQueryable().Where(e => e.UserId == gId).FirstOrDefaultAsync(cancellationToken);
	}

	public Task<bool> IsUserExistsAsync(string id, CancellationToken cancellationToken)
	{
		var gId = new Guid(id);

		return _dbContext.Users.AnyAsync(e => e.UserId == gId, cancellationToken);
	}
}