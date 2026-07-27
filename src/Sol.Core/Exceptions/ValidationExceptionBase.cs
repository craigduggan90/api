using FluentValidation.Results;

namespace Sol.Core.Exceptions;

public abstract class ValidationExceptionBase(IEnumerable<ValidationFailure> errors)
    : Exception(ExceptionMessage)
{
    internal const string ExceptionMessage = "One or more validation errors occurred.";

    public IEnumerable<ValidationFailure> Errors { get; } = errors;
}