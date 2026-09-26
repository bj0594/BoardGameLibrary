using System.Data.Common;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using BoardGameLibrary.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace BoardGameLibrary.Tests;

public sealed class BoardGameApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");

    public FakeBoardGameGeekClient FakeBggClient { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<BoardGameDbContext>>();
            services.RemoveAll<IBoardGameGeekClient>();

            connection.Open();

            services.AddSingleton<DbConnection>(connection);
            services.AddDbContext<BoardGameDbContext>(options => options.UseSqlite(connection));
            services.AddSingleton<IBoardGameGeekClient>(FakeBggClient);
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        dbContext.Database.EnsureCreated();

        return host;
    }

    public void BreakDatabaseConnection()
    {
        connection.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            connection.Dispose();
        }

        base.Dispose(disposing);
    }
}

public enum FakeBggFailureMode
{
    None,
    NotFound,
    HttpRequestException
}

public sealed class FakeBoardGameGeekClient : IBoardGameGeekClient
{
    public FakeBggFailureMode FailureMode { get; set; }

    public int CallCount { get; private set; }

    public Task<BoardGame?> GetBoardGameAsync(
        int bggId,
        CancellationToken cancellationToken = default)
    {
        CallCount++;

        return FailureMode switch
        {
            FakeBggFailureMode.NotFound => Task.FromResult<BoardGame?>(null),
            FakeBggFailureMode.HttpRequestException => throw new HttpRequestException("Controlled BGG failure."),
            _ => Task.FromResult<BoardGame?>(CreateGame(bggId))
        };
    }

    private static BoardGame CreateGame(int bggId)
    {
        return new BoardGame
        {
            BggId = bggId,
            Title = "Test Game",
            MinPlayers = 1,
            MaxPlayers = 4,
            BggAverageRating = 8.2,
            BggRatingCount = 1000,
            PlayerCountRecommendations =
            [
                new PlayerCountRecommendation
                {
                    PlayerCount = "1",
                    BestVotes = 20,
                    RecommendedVotes = 15,
                    NotRecommendedVotes = 5
                },
                new PlayerCountRecommendation
                {
                    PlayerCount = "2",
                    BestVotes = 30,
                    RecommendedVotes = 20,
                    NotRecommendedVotes = 2
                },
                new PlayerCountRecommendation
                {
                    PlayerCount = "4+",
                    BestVotes = 12,
                    RecommendedVotes = 8,
                    NotRecommendedVotes = 3
                }
            ]
        };
    }
}
