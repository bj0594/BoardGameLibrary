using System.Data.Common;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using BoardGameLibrary.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

public sealed class BoardGameApiFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection connection = new SqliteConnection("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var dbOptions = services.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(DbContextOptions<BoardGameDbContext>));

            if (dbOptions is not null)
            {
                services.Remove(dbOptions);
            }

            connection.Open();
            services.AddSingleton(connection);
            services.AddDbContext<BoardGameDbContext>(options => options.UseSqlite(connection));

            var bggClient = services.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(IBoardGameGeekClient));

            if (bggClient is not null)
            {
                services.Remove(bggClient);
            }

            services.AddScoped<IBoardGameGeekClient, FakeBoardGameGeekClient>();
        });
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

public sealed class FakeBoardGameGeekClient : IBoardGameGeekClient
{
    public Task<BoardGame?> GetBoardGameAsync(
        int bggId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<BoardGame?>(new BoardGame
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
                }
            ]
        });
    }
}
