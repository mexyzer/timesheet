using FluentValidation;
using Timesheet.Core.Interfaces;

namespace Timesheet.WebApi.EndPoints.RoleManagement;

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
	private readonly IUserService _service;

	public CreateRoleRequestValidator(IUserService service)
	{
		RuleLevelCascadeMode = CascadeMode.Stop;

		_service = service;

		RuleFor(e => e.Name).NotNull().NotEmpty().MaximumLength(256);
		RuleFor(e => e.PermissionIds).NotNull().NotEmpty()
			.Must(e => e!.Length > 0)
			.Must(NotDuplicate);
		RuleForEach(e => e.PermissionIds).NotNull().Must(IsGuid)
			.MustAsync(PermissionExists);
	}

	private async Task<bool> PermissionExists(string arg1, CancellationToken arg2)
	{
		var permission = await _service.GetPermissionByIdAsync(arg1, arg2);

		return permission != null;
	}

	private static bool NotDuplicate(string[]? arg)
	{
		if (arg == null || arg.Length == 0)
		{
			return false;
		}

		var length = arg.Length;
		var lengthAfter = arg.Distinct().Count();

		return length == lengthAfter;
	}

	private bool IsGuid(string arg)
	{
		return Guid.TryParse(arg, out _);
	}
}