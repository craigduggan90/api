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

builder.Services.AddControllers();
builder.Services.AddRouting();

var app = builder.Build();

app.UseSwaggerDocumentation();

app.UseRouting();
app.MapControllers();

app.Run();
