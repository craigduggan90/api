using Sol.Data;

var builder = WebApplication.CreateBuilder(args);
builder
    .AddDataServices();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
