using FluentValidation;
using Timesheet.Core.Interfaces;

namespace Timesheet.WebApi.EndPoints.RoleManagement;

public class EditRoleRequestValidator : AbstractValidator<EditRoleRequest>
{
	public EditRoleRequestValidator(IUserService userService)
	{
		RuleLevelCascadeMode = CascadeMode.Stop;

		RuleFor(e => e.RoleId).Must(IsGuid);

		RuleFor(e => e.Dto).NotNull()
			.SetValidator(new EditRoleDtoValidator(userService)!);
	}

	private bool IsGuid(string arg)
	{
		return Guid.TryParse(arg, out _);
	}
}

public class EditRoleDtoValidator : AbstractValidator<EditRoleDto>
{
	private readonly IUserService _userService;

	public EditRoleDtoValidator(IUserService userService)
	{
		RuleLevelCascadeMode = CascadeMode.Stop;

		_userService = userService;

		RuleFor(e => e.PermissionIds).NotNull().NotEmpty()
			.Must(e => e!.Length > 0)
			.Must(NotDuplicate);
		RuleForEach(e => e.PermissionIds).NotNull().Must(IsGuid)
			.MustAsync(PermissionExists);
	}

	private async Task<bool> PermissionExists(string arg1, CancellationToken arg2)
	{
		var permission = await _userService.GetPermissionByIdAsync(arg1, arg2);

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