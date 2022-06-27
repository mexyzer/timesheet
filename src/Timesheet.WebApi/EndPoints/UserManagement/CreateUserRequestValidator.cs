using FluentValidation;
using Timesheet.Core.Interfaces;

// ReSharper disable All

namespace Timesheet.WebApi.EndPoints.UserManagement;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
	private readonly IUserService _userService;

	public CreateUserRequestValidator(IUserService userService)
	{
		RuleLevelCascadeMode = CascadeMode.Stop;

		_userService = userService;

		RuleFor(e => e.Username).NotNull().NotEmpty().EmailAddress();
		RuleFor(e => e.Password).NotNull().NotEmpty();
		RuleFor(e => e.ConfirmPassword).Equal(e => e.Password);
		RuleFor(e => e.FirstName).NotNull().NotEmpty();
		RuleFor(e => e.RoleIds).NotNull().Must(e => e!.Length > 0).Must(NotDuplicate);
		RuleForEach(e => e.RoleIds).NotNull().Must(IsGuid).MustAsync(RoleExists);
	}

	private bool NotDuplicate(string[]? arg)
	{
		if (arg == null || arg.Length == 0)
		{
			return false;
		}

		var length = arg!.Length;
		var lengthAfter = arg!.Distinct().Count();

		return length == lengthAfter;
	}

	private bool IsGuid(string roleId)
	{
		return Guid.TryParse(roleId, out _);
	}

	private async Task<bool> RoleExists(string roleId, CancellationToken cancellationToken)
	{
		var role = await _userService.GetRoleByIdAsync(roleId, cancellationToken);

		return role != null;
	}
}