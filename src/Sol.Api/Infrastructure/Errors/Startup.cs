using Sol.Api.Infrastructure.Errors.Handlers;
using System.Diagnostics.CodeAnalysis;

namespace Sol.Api.Infrastructure.Errors;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static IHostApplicationBuilder AddErrorHandling(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddExceptionHandler<MissingHeaderExceptionHandler>()
            .AddExceptionHandler<NotFoundExceptionHandler>()
            .AddExceptionHandler<ValidationExceptionHandler>()
            .AddExceptionHandler<ConcurrencyTokenMismatchExceptionHandler>()
            .AddExceptionHandler<UnhandledExceptionHandler>()
            .AddProblemDetails();

        return builder;
    }

    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        => app.UseExceptionHandler();
}