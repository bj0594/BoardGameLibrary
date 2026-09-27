using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

/// <summary>Verifies that the Development seed is complete and idempotent.</summary>
public class SeedDataTests
{
    [Fact]
    public async Task Seed_IsIdempotent_AndCreatesCompleteDemoLibrary()
    {
        using var factory = new BoardGameApiFactory();

        await BoardGameSeeder.SeedAsync(factory.Services);
        await BoardGameSeeder.SeedAsync(factory.Services);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        var games = await dbContext.BoardGames
            .Include(game => game.PlayerCountRatings)
            .AsNoTracking()
            .ToListAsync();

        Assert.Equal(10, games.Count);
        Assert.Equal(10, games.Select(game => game.BggId).Distinct().Count());
        Assert.All(games, game => Assert.NotEmpty(game.PlayerCountRatings));

        Assert.All(games, game =>
        {
            Assert.All(game.PlayerCountRatings, rating =>
            {
                Assert.InRange(rating.PlayerCount, game.MinPlayers, game.MaxPlayers);
                Assert.InRange(rating.Rating, 0m, 10m);
                Assert.Equal(decimal.Round(rating.Rating, 1), rating.Rating);
            });
        });
    }
}
