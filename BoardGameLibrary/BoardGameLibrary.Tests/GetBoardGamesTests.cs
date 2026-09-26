using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

/// <summary>
/// Verifies read-only collection and single-resource retrieval through the HTTP boundary.
/// </summary>
public class GetBoardGamesTests
{
    [Fact]
    public async Task GetAll_WithStoredGames_ReturnsAllGamesInStableOrder()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, new BoardGame
        {
            Title = "Zeta",
            MinPlayers = 2,
            MaxPlayers = 4,
            MinPlayTimeMinutes = 60,
            MaxPlayTimeMinutes = 90,
            CreatedAt = DateTimeOffset.UtcNow
        });
        await SeedAsync(factory, new BoardGame
        {
            Title = "Alpha",
            MinPlayers = 1,
            MaxPlayers = 2,
            MinPlayTimeMinutes = 20,
            MaxPlayTimeMinutes = 30,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var response = await client.GetAsync("/api/games");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        Assert.Equal(["Alpha", "Zeta"], games!.Select(game => game.Title));
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ReturnsEmptyCollection()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        Assert.Empty(games!);
    }

    [Fact]
    public async Task GetById_WithPlayerCount_ReturnsSelectedPlayerRating()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var id = await SeedAsync(factory, new BoardGame
        {
            Title = "Test Game",
            MinPlayers = 1,
            MaxPlayers = 4,
            MinPlayTimeMinutes = 30,
            MaxPlayTimeMinutes = 60,
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings =
            [
                new() { PlayerCount = 2, Rating = 8.0m },
                new() { PlayerCount = 4, Rating = 9.0m }
            ]
        });

        var response = await client.GetAsync($"/api/games/{id}?players=4");
        var game = await response.Content.ReadFromJsonAsync<BoardGame>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(game);
        Assert.Equal(4, game!.SelectedPlayerCount);
        Assert.Equal(9.0m, game.SelectedPlayerRating);
    }

    [Fact]
    public async Task GetById_WhenGameDoesNotExist_ReturnsNotFound()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static async Task<int> SeedAsync(BoardGameApiFactory factory, BoardGame game)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        dbContext.BoardGames.Add(game);
        await dbContext.SaveChangesAsync();
        return game.Id;
    }
}
