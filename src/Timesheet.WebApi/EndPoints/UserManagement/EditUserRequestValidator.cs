using FluentValidation;
using Timesheet.Core.Interfaces;

namespace Timesheet.WebApi.EndPoints.UserManagement;

public class EditUserRequestValidator : AbstractValidator<EditUserRequest>
{
	private readonly IUserService _roleService;

	public EditUserRequestValidator(IUserService roleService)
	{
		RuleLevelCascadeMode = CascadeMode.Stop;

		_roleService = roleService;

		RuleFor(e => e.UserId).NotNull().NotEmpty().Must(IsGuid);
		RuleFor(e => e.FirstName).NotNull().NotEmpty();
		RuleFor(e => e.RoleIds).NotNull().Must(e => e!.Length > 0).Must(NotDuplicate);
		RuleForEach(e => e.RoleIds).NotNull().Must(IsGuid).MustAsync(RoleExists);
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

	private bool IsGuid(string? roleId)
	{
		return Guid.TryParse(roleId!, out _);
	}

	private async Task<bool> RoleExists(string roleId, CancellationToken cancellationToken)
	{
		var role = await _roleService.GetRoleByIdAsync(roleId, cancellationToken);

		return role != null;
	}
}