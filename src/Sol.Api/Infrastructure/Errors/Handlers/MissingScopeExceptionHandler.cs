using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sol.Api.Exceptions;

namespace Sol.Api.Infrastructure.Errors.Handlers;

public class MissingScopeExceptionHandler : IExceptionHandler
{
    private const string Title = "Forbidden";
    private const string Type = "https://www.sol.api/errors/missing-scope";
    internal const int StatusCode = 403;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not MissingScopeException typedException)
            return false;

        httpContext.Response.StatusCode = StatusCode;
        await httpContext.Response.WriteAsJsonAsync(GetProblemDetails(typedException), cancellationToken);
        return true;
    }

    internal static ProblemDetails GetProblemDetails(MissingScopeException exception) =>
        new()
        {
            Title = Title,
            Detail = exception.Message,
            Type = Type,
            Status = StatusCode
        };
}