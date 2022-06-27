using System.Net;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Timesheet.WebApi.EndPoints.HealthChecks;

public class Status : EndpointBaseSync.WithoutRequest.WithActionResult<string>
{
    [HttpGet("api/healthchecks/status")]
    [SwaggerResponse((int)HttpStatusCode.OK, "", typeof(string))]
    [SwaggerOperation(
        Summary = "API Status",
        OperationId = "HealthChecks.Status",
        Tags = new[] {"HealthChecks"})
    ]
    public override ActionResult<string> Handle()
    {
        return Ok("API Server is running..");
    }
}