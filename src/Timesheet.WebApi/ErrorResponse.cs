namespace Timesheet.WebApi;

public class ErrorResponse
{
    public string Message { get; init; } = string.Empty;
    public object? Payload { get; init; }
}

public static class ErrorResponseExtension
{
    public static ErrorResponse Create(string message)
    {
        return new ErrorResponse {Message = message};
    }

    public static ErrorResponse Create(string message, object? payload)
    {
        return new ErrorResponse {Message = message, Payload = payload};
    }
}