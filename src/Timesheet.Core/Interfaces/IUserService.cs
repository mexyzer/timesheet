using Timesheet.Core.Entities;

namespace Timesheet.Core.Interfaces;

public interface IUserService
{
	Task<IReadOnlyList<User>> GetAllUserAsync(int page, int size, CancellationToken cancellationToken);
	Task<User?> GetUserByIdAsync(string id, CancellationToken cancellationToken);
	Task<bool> IsUserExistsAsync(string id, CancellationToken cancellationToken);
	Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
	Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken);
	Task<bool> IsEmailAddressExists(string email, CancellationToken cancellationToken);

	Task<IReadOnlyList<Role>> GetAllRoleAsync(int page, int size, CancellationToken cancellationToken);
	Task<Role?> GetRoleByIdAsync(string id, CancellationToken cancellationToken);
	Task<bool> IsRoleExistsByNameAsync(string name, CancellationToken cancellationToken);

	Task<IReadOnlyList<Permission>> GetAllPermissionAsync(int page, int size, CancellationToken cancellationToken);
	Task<Permission?> GetPermissionByIdAsync(string id, CancellationToken cancellationToken);
	Task<bool> IsPermissionExistsByIdAsync(string id, CancellationToken cancellationToken);
	Task<Permission?> GetPermissionByNameAsync(string name, CancellationToken cancellationToken);
}