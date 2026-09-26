using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

/// <summary>Verifies inclusive player-count filtering on the collection GET endpoint.</summary>
public class PlayerCountDiscoveryTests
{
    [Fact]
    // One player is the primary discovery case and should match inclusive ranges containing 1.
    public async Task GetByPlayerCount_ForOnePlayer_ReturnsOnlyCompatibleGames()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, new BoardGame
        {
            Title = "Solo Game",
            MinPlayers = 1,
            MaxPlayers = 4,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await SeedAsync(factory, new BoardGame
        {
            Title = "Two Player Game",
            MinPlayers = 2,
            MaxPlayers = 4,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var response = await client.GetAsync("/api/games?players=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.NotNull(games);
        Assert.Single(games!);
        Assert.Equal("Solo Game", games[0].Title);
    }

    [Fact]
    // Mid-range filtering proves the query is based on both minimum and maximum players.
    public async Task GetByPlayerCount_ForTwoPlayers_ReturnsGamesThatSupportTwoPlayers()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, new BoardGame
        {
            Title = "One To Four",
            MinPlayers = 1,
            MaxPlayers = 4,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await SeedAsync(factory, new BoardGame
        {
            Title = "Three To Four",
            MinPlayers = 3,
            MaxPlayers = 4,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var response = await client.GetAsync("/api/games?players=2");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        Assert.Single(games!);
        Assert.Equal("One To Four", games[0].Title);
    }

    [Fact]
    // The maximum boundary is inclusive: a game supporting four players matches players=4.
    public async Task GetByPlayerCount_ForMaxPlayers_ReturnsGamesSupportingBoundaryValue()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, new BoardGame
        {
            Title = "Supports Four",
            MinPlayers = 1,
            MaxPlayers = 4,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await SeedAsync(factory, new BoardGame
        {
            Title = "Supports Three",
            MinPlayers = 1,
            MaxPlayers = 3,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var response = await client.GetAsync("/api/games?players=4");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        Assert.Single(games!);
        Assert.Equal("Supports Four", games[0].Title);
    }

    [Fact]
    // A valid query with no matches is still a successful read, not an error.
    public async Task GetByPlayerCount_WhenNoGamesMatch_ReturnsEmptyCollection()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, new BoardGame
        {
            Title = "Test Game",
            MinPlayers = 1,
            MaxPlayers = 2,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var response = await client.GetAsync("/api/games?players=4");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        Assert.Empty(games!);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("abc")]
    // Invalid query values are rejected before they can become database work.
    public async Task GetByPlayerCount_WithInvalidValue_ReturnsBadRequest(string value)
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/games?players={value}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Seed through EF to isolate the query behaviour from POST validation.
    private static async Task SeedAsync(
        BoardGameApiFactory factory,
        BoardGame game)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        dbContext.BoardGames.Add(game);
        await dbContext.SaveChangesAsync();
    }
}
