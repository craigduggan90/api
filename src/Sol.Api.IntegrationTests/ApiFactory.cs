using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sol.Data.Context;

namespace Sol.Api.IntegrationTests;

public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _keepAliveConnection = new($"Data Source=file:{Guid.NewGuid():N};Mode=Memory;Cache=Shared");

    public ApiFactory() => _keepAliveConnection.Open();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Reader"] = _keepAliveConnection.ConnectionString,
                ["ConnectionStrings:Writer"] = _keepAliveConnection.ConnectionString
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<ApiDbContext>().Database.Migrate();

        return host;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _keepAliveConnection.Dispose();

        base.Dispose(disposing);
    }
}