using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Timesheet.Core.Interfaces;

namespace Timesheet.WebApi.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public AuthorizeAttribute()
    {
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var service = context.HttpContext.RequestServices.GetService<ICurrentUserService>()!;

        //not logged in
        if (service.UserId == null)
        {
            context.Result =
                new JsonResult(new ErrorResponse {Message = "Unauthorized"})
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            return;
        }
    }
}