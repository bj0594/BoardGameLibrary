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
/// Creates a real API host backed by one isolated in-memory SQLite database per factory instance.
/// </summary>
public sealed class BoardGameApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");

    /// <summary>Replaces the production database registration with the isolated test database.</summary>
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

    /// <summary>
    /// Creates the schema once the test host is ready. This is test-only setup, not application startup logic.
    /// </summary>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        dbContext.Database.EnsureCreated();

        return host;
    }

    /// <summary>
    /// Deliberately breaks the shared test connection so failure-handling tests can exercise a database error.
    /// </summary>
    public void BreakDatabaseConnection()
    {
        connection.Dispose();
    }

    /// <summary>Releases the test database connection after the factory is disposed.</summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            connection.Dispose();
        }

        base.Dispose(disposing);
    }
}
