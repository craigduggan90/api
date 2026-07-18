using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Sol.Api.Swagger.Examples.V1.Jobs;
using Swashbuckle.AspNetCore.Filters;

namespace Sol.Api.Swagger;

public static class Startup
{
    public static WebApplicationBuilder AddSwaggerDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sol API",
                Version = "v1",
                Description = "A facade API for enqueuing and tracking long-running jobs."
            });
            options.ExampleFilters();
        });

        builder.Services.AddSwaggerExamplesFromAssemblyOf<JobResponseDetailModelExample>();

        return builder;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sol API v1"));

        return app;
    }
}