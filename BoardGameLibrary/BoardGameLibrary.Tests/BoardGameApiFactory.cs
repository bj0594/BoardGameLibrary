using System.Data.Common;
using BoardGameLibrary.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace BoardGameLibrary.Tests;

/// <summary>
/// Creates an isolated in-memory SQLite database for each API integration test.
/// The production database and Development seed data are intentionally not used here.
/// </summary>
public sealed class BoardGameApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<BoardGameDbContext>>();

            connection.Open();
            services.AddSingleton<DbConnection>(connection);
            services.AddDbContext<BoardGameDbContext>(options => options.UseSqlite(connection));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        dbContext.Database.Migrate();

        return host;
    }

    /// <summary>Forces the controlled database dependency to fail on the next database operation.</summary>
    public void BreakDatabaseConnection() => connection.Dispose();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            connection.Dispose();
        }

        base.Dispose(disposing);
    }
}
