using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Timesheet.WebApi;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ApiExceptionFilterAttribute>? _logger;

    public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute>? logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        const string msg = "An error occurred while processing your request.";

        var ex = context.Exception;

        _logger?.LogError(ex, msg);

        context.Result =
            new ObjectResult(new ErrorResponse {Message = msg}) {StatusCode = StatusCodes.Status500InternalServerError};

        context.ExceptionHandled = true;

        base.OnException(context);
    }
}