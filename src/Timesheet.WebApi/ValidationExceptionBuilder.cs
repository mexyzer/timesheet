using FluentValidation.Results;

namespace Timesheet.WebApi;

public class ValidationExceptionBuilder
{
    public ValidationExceptionBuilder()
    {
        Failures = new Dictionary<string, ValidationExceptionResult>();
    }

    public IDictionary<string, ValidationExceptionResult> Failures { get; }

    public static ErrorResponse Build(List<ValidationFailure> failures)
    {
        var builder = new ValidationExceptionBuilder();

        var propertyNames = failures
            .Select(e => e.PropertyName)
            .Distinct();

        foreach (var item in propertyNames)
        {
            var vm = new ValidationExceptionResult();

            vm.AttemptedValue = failures.Where(x => x.PropertyName == item)
                .Select(x => x.AttemptedValue)
                .First();

            vm.ErrorMessage = failures.Where(e => e.PropertyName == item)
                .Select(e => e.ErrorMessage)
                .ToArray();

            builder.Failures.Add(item, vm);
        }

        return new ErrorResponse()
        {
            Message = "One or more validation failures have occurred.", Payload = builder.Failures
        };
    }
}

public class ValidationExceptionResult
{
    public object AttemptedValue { get; set; } = new();

    public string[] ErrorMessage { get; set; } = {""};
}