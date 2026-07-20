using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sol.Api.Swagger.Examples.V1.Jobs;
using Swashbuckle.AspNetCore.Filters;

namespace Sol.Api.Swagger;

public static class Startup
{
    public static WebApplicationBuilder AddSwaggerDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.ConfigureOptions<VersionedSwaggerOptions>();
        builder.Services.AddSwaggerGen(options => options.ExampleFilters());
        builder.Services.AddSwaggerExamplesFromAssemblyOf<JobResponseDetailModelExample>();

        return builder;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"Sol API {description.GroupName}");
            }
        });

        return app;
    }
}