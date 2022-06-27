using Timesheet.SharedKernel;

namespace Timesheet.Core.Entities;

public class Permission : BaseEntity
{
	public Permission()
	{
		PermissionId = Guid.NewGuid();
	}

	public Guid PermissionId { get; set; }
	public string Name { get; set; } = string.Empty;
}