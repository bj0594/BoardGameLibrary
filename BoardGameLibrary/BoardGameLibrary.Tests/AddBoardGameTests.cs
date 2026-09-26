using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

/// <summary>Verifies successful resource creation, persistence, and REST creation semantics.</summary>
public class AddBoardGameTests
{
    [Fact]
    // Proves the complete POST -> validation -> service -> EF Core -> SQLite path.
    public async Task CreateValidGame_PersistsAndReturnsCreated()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var request = new CreateBoardGameRequest
        {
            Title = "Test Game",
            MinPlayers = 1,
            MaxPlayers = 4
        };

        var response = await client.PostAsJsonAsync("/api/games", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var createdGame = await response.Content.ReadFromJsonAsync<BoardGame>();

        Assert.NotNull(createdGame);
        Assert.True(createdGame!.Id > 0);
        Assert.Equal("Test Game", createdGame.Title);
        Assert.Equal(1, createdGame.MinPlayers);
        Assert.Equal(4, createdGame.MaxPlayers);
        Assert.Equal(TimeSpan.Zero, createdGame.CreatedAt.Offset);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        var persistedGame = await dbContext.BoardGames.FindAsync(createdGame.Id);

        Assert.NotNull(persistedGame);
        Assert.Equal("Test Game", persistedGame!.Title);
    }

    [Fact]
    // CreatedAtAction must point to an actually retrievable resource, not merely contain a plausible URL.
    public async Task CreateValidGame_LocationPointsToCreatedResource()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new
            {
                title = "Location Test",
                minPlayers = 1,
                maxPlayers = 2
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var createdGame = await response.Content.ReadFromJsonAsync<BoardGame>();

        Assert.NotNull(createdGame);

        var getResponse = await client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedGame = await getResponse.Content.ReadFromJsonAsync<BoardGame>();
        Assert.NotNull(retrievedGame);
        Assert.Equal(createdGame!.Id, retrievedGame!.Id);
        Assert.Equal(createdGame.Title, retrievedGame.Title);
    }
}
