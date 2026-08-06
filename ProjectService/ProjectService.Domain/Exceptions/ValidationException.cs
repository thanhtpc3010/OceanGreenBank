using FluentValidation.Results;

namespace ProjectService.Domain.Exceptions;

/// <summary>
/// Ném ra khi một hoặc nhiều rule validation (FluentValidation) thất bại.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException() : base("One or more validation failures have occurred.")
    {
        Errors = [];
        Failures = [];
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Failures = failures.ToList();
        Errors = failures.Select(x => x.ErrorMessage).ToList();
    }

    public List<string> Errors { get; }

    public List<ValidationFailure> Failures { get; }
}
