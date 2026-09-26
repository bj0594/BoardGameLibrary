using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;
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
    public async Task CreateValidGame_PersistsEnrichedResourceAndReturnsCreated()
    {
        // Arrange
        var request = new CreateBoardGameRequest
        {
            BggId = 12345
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/games", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdGame = await response.Content.ReadFromJsonAsync<BoardGame>();

        Assert.NotNull(createdGame);
        Assert.Equal(12345, createdGame!.BggId);
        Assert.Equal("Test Game", createdGame.Title);
        Assert.Single(createdGame.PlayerCountRecommendations);
        Assert.Equal("1", createdGame.PlayerCountRecommendations[0].PlayerCount);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

        var persistedGame = await dbContext.BoardGames.FindAsync(12345);
        Assert.NotNull(persistedGame);
        Assert.Equal("Test Game", persistedGame!.Title);

        var persistedRecommendation = await dbContext.PlayerCountRecommendations
            .SingleAsync(recommendation => recommendation.BoardGameId == 12345);

        Assert.Equal("1", persistedRecommendation.PlayerCount);
        Assert.Equal(20, persistedRecommendation.BestVotes);
        Assert.Equal(15, persistedRecommendation.RecommendedVotes);
        Assert.Equal(5, persistedRecommendation.NotRecommendedVotes);
    }
}
