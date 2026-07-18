using Sol.Api.Infrastructure.Errors.Handlers;

namespace Sol.Api.Infrastructure.Errors;

public static class Startup
{
    public static IHostApplicationBuilder AddErrorHandling(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddExceptionHandler<NotFoundExceptionHandler>()
            .AddExceptionHandler<ValidationExceptionHandler>()
            .AddExceptionHandler<UnhandledExceptionHandler>()
            .AddProblemDetails();

        return builder;
    }
    
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        => app.UseExceptionHandler();
}