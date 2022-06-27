using FluentValidation;

namespace Timesheet.WebApi.EndPoints.Users;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
	public LoginRequestValidator()
	{
		RuleLevelCascadeMode = CascadeMode.Stop;

		RuleFor(e => e.Username).NotNull().NotEmpty().EmailAddress();
		RuleFor(e => e.Password).NotNull().NotEmpty();
	}
}