using Sol.Api.Swagger;
using Sol.Common;
using Sol.Core;
using Sol.Data;

var builder = WebApplication.CreateBuilder(args);
builder
    .AddCommonServices()
    .AddCoreServices()
    .AddDataServices()
    .AddSwaggerDocumentation();

var app = builder.Build();

app.UseSwaggerDocumentation();

app.MapGet("/", () => "Hello World!");

app.Run();
