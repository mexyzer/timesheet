using Timesheet.SharedKernel;

namespace Timesheet.Core.Entities;

public class Role : BaseEntity
{
	public Role()
	{
		RoleId = Guid.NewGuid();

		RolePermissions = new HashSet<RolePermission>();
	}

	public Guid RoleId { get; set; }
	public string Name { get; set; } = String.Empty;
	public string NormalizedName { get; set; } = string.Empty;

	public ICollection<RolePermission> RolePermissions { get; set; }
}