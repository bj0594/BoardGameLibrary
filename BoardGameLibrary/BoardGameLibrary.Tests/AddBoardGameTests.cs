using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

/// <summary>
/// Proves the complete POST create flow, including nested player-count ratings.
/// </summary>
public class AddBoardGameTests
{
    [Fact]
    public async Task CreateValidGame_PersistsAndReturnsCreated()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/games", new
        {
            title = "Test Game",
            minPlayers = 1,
            maxPlayers = 4,
            minPlayTimeMinutes = 45,
            maxPlayTimeMinutes = 90,
            playerRatings = new[]
            {
                new { playerCount = 1, rating = 7.5m },
                new { playerCount = 2, rating = 8.5m },
                new { playerCount = 4, rating = 8.0m }
            }
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var createdGame = await response.Content.ReadFromJsonAsync<BoardGame>();

        Assert.NotNull(createdGame);
        Assert.True(createdGame!.Id > 0);
        Assert.Equal("Test Game", createdGame.Title);
        Assert.Equal(45, createdGame.MinPlayTimeMinutes);
        Assert.Equal(90, createdGame.MaxPlayTimeMinutes);
        Assert.Equal(3, createdGame.PlayerCountRatings.Count);
        Assert.True(createdGame.CreatedAt > DateTimeOffset.MinValue);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        var persistedGame = await dbContext.BoardGames
            .Include(game => game.PlayerCountRatings)
            .SingleAsync(game => game.Id == createdGame.Id);

        Assert.Equal("Test Game", persistedGame.Title);
        Assert.Contains(persistedGame.PlayerCountRatings, rating =>
            rating.PlayerCount == 2 && rating.Rating == 8.5m);
    }

    [Fact]
    public async Task CreateValidGame_LocationPointsToCreatedResource()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/games", new
        {
            title = "Location Test",
            minPlayers = 1,
            maxPlayers = 2,
            minPlayTimeMinutes = 20,
            maxPlayTimeMinutes = 30
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var createdGame = await response.Content.ReadFromJsonAsync<BoardGame>();
        Assert.NotNull(createdGame);

        var locationResponse = await client.GetAsync(response.Headers.Location);

        Assert.Equal(HttpStatusCode.OK, locationResponse.StatusCode);
        var locatedGame = await locationResponse.Content.ReadFromJsonAsync<BoardGame>();

        Assert.NotNull(locatedGame);
        Assert.Equal(createdGame!.Id, locatedGame!.Id);
    }
}
