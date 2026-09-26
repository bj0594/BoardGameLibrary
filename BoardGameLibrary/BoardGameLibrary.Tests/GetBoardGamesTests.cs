using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

/// <summary>Verifies read-only collection and single-resource GET behaviour.</summary>
public class GetBoardGamesTests
{
    [Fact]
    // Ordering is explicit in the API contract so clients receive deterministic results.
    public async Task GetAll_WithStoredGames_ReturnsAllGamesInStableOrder()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, new BoardGame
        {
            Title = "Zeta",
            MinPlayers = 2,
            MaxPlayers = 4,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await SeedAsync(factory, new BoardGame
        {
            Title = "Alpha",
            MinPlayers = 1,
            MaxPlayers = 2,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var response = await client.GetAsync("/api/games");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.NotNull(games);
        Assert.Equal(["Alpha", "Zeta"], games!.Select(game => game.Title));
    }

    [Fact]
    // A collection endpoint should represent an empty resource collection with 200, not 404.
    public async Task GetAll_WhenEmpty_ReturnsEmptyCollection()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.NotNull(games);
        Assert.Empty(games!);
    }

    [Fact]
    // Proves a known identifier returns the complete persisted resource.
    public async Task GetById_WhenGameExists_ReturnsGame()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var seededId = await SeedAsync(factory, new BoardGame
        {
            Title = "Test Game",
            MinPlayers = 1,
            MaxPlayers = 4,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var response = await client.GetAsync($"/api/games/{seededId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var game = await response.Content.ReadFromJsonAsync<BoardGame>();

        Assert.NotNull(game);
        Assert.Equal(seededId, game!.Id);
        Assert.Equal("Test Game", game.Title);
    }

    [Fact]
    // A missing single resource is a resource-level 404 rather than a collection-level empty result.
    public async Task GetById_WhenGameDoesNotExist_ReturnsNotFound()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static async Task<int> SeedAsync(
        BoardGameApiFactory factory,
        BoardGame game)
    {
        // Seed directly through EF so GET tests focus on retrieval rather than also testing POST.
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        dbContext.BoardGames.Add(game);
        await dbContext.SaveChangesAsync();
        return game.Id;
    }
}
