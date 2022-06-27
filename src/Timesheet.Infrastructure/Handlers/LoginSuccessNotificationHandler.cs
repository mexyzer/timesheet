using MediatR;
using Microsoft.Extensions.Logging;
using Timesheet.Core.Events;

namespace Timesheet.Infrastructure.Handlers;

public class LoginSuccessNotificationHandler : INotificationHandler<LoginSuccessEvent>
{
	private readonly ILogger<LoginSuccessNotificationHandler>? _logger;

	public LoginSuccessNotificationHandler(ILogger<LoginSuccessNotificationHandler>? logger)
	{
		_logger = logger;
	}

	public Task Handle(LoginSuccessEvent notification, CancellationToken cancellationToken)
	{
		_logger?.LogInformation($"Event {nameof(LoginSuccessEvent)} fired");

		return Task.CompletedTask;
	}
}