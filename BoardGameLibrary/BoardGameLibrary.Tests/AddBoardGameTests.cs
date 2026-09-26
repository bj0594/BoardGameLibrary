using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

public class AddBoardGameTests : IClassFixture<BoardGameApiFactory>
{
    private readonly HttpClient client;
    private readonly BoardGameApiFactory factory;

    public AddBoardGameTests(BoardGameApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateValidGame_PersistsAndReturnsCreated()
    {
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
        Assert.True(createdGame.CreatedAt > DateTimeOffset.MinValue);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        var persistedGame = await dbContext.BoardGames.FindAsync(createdGame.Id);

        Assert.NotNull(persistedGame);
        Assert.Equal("Test Game", persistedGame!.Title);
    }

    [Fact]
    public async Task CreateValidGame_LocationPointsToCreatedResource()
    {
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
        Assert.Contains($"/api/games/{createdGame!.Id}", response.Headers.Location!.ToString());
    }
}
