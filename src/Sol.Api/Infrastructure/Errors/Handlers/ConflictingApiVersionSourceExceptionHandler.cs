using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sol.Api.Exceptions;

namespace Sol.Api.Infrastructure.Errors.Handlers;

public class ConflictingApiVersionSourceExceptionHandler : IExceptionHandler
{
    private const string Title = "Bad Request";
    private const string Type = "https://www.sol.api/errors/conflicting-api-version-source";
    internal const int StatusCode = 400;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is not ConflictingApiVersionSourceException typedException)
            return false;

        httpContext.Response.StatusCode = StatusCode;
        await httpContext.Response.WriteAsJsonAsync(GetProblemDetails(typedException), cancellationToken);
        return true;
    }

    internal static ProblemDetails GetProblemDetails(ConflictingApiVersionSourceException exception) =>
        new()
        {
            Title = Title,
            Detail = exception.Message,
            Type = Type,
            Status = StatusCode
        };
}