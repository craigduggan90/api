using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sol.Common.Services;
using System.Diagnostics.CodeAnalysis;

namespace Sol.Common;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder AddCommonServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IEnvironmentVariableAccessor, EnvironmentVariableAccessor>();
        return builder;
    }
}