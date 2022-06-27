using System.Reflection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.Core.Interfaces;
using Timesheet.Infrastructure.Services;

namespace Timesheet.Infrastructure;

public static class InfrastructureServiceCollection
{
	public static IServiceCollection AddInfrastructureService(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddMediatR(Assembly.GetExecutingAssembly());

		services.AddScoped<IEmailSender, SmtpEmailSender>();

		services.AddScoped<IDateTime, DateTimeService>();

		services.AddScoped<IUserService, DefaultUserService>();

		return services;
	}
}