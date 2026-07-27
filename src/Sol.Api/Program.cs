using Sol.Api.Infrastructure.Errors;
using Sol.Api.Infrastructure.Versioning;
using Sol.Api.Swagger;
using Sol.Common;
using Sol.Core;
using Sol.Data;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);
builder
    .AddCommonServices()
    .AddCoreServices()
    .AddDataServices()
    .AddVersioning()
    .AddSwaggerDocumentation()
    .AddErrorHandling();

builder.Services.AddControllers();
builder.Services.AddRouting();

var app = builder.Build();

app.UseSwaggerDocumentation();
app.UseErrorHandling();

app.UseRouting();
app.MapControllers();

app.Run();

/// <summary>This partial class is required for testing and exclusion from test coverage metrics.</summary>
[ExcludeFromCodeCoverage]
public partial class Program;