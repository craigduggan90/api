using Sol.Api.Infrastructure.Errors;
using Sol.Api.Swagger;
using Sol.Common;
using Sol.Core;
using Sol.Data;

var builder = WebApplication.CreateBuilder(args);
builder
    .AddCommonServices()
    .AddCoreServices()
    .AddDataServices()
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
