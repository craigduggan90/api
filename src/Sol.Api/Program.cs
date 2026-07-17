using Sol.Common;
using Sol.Core;
using Sol.Data;

var builder = WebApplication.CreateBuilder(args);
builder
    .AddCommonServices()
    .AddCoreServices()
    .AddDataServices();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
