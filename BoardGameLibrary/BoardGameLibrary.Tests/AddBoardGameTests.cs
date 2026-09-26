using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

public class AddBoardGameTests
{
    [Fact]
    public async Task CreateValidGame_PersistsEnrichedResourceAndReturnsCreated()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var request = new CreateBoardGameRequest
        {
            BggId = 12345
        };

        var response = await client.PostAsJsonAsync("/api/games", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/games/12345", response.Headers.Location?.AbsolutePath);

        var createdGame = await response.Content.ReadFromJsonAsync<BoardGame>();

        Assert.NotNull(createdGame);
        Assert.Equal(12345, createdGame!.BggId);
        Assert.Equal("Test Game", createdGame.Title);
        Assert.Single(createdGame.PlayerCountRecommendations);
        Assert.Equal("1", createdGame.PlayerCountRecommendations[0].PlayerCount);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

        var persistedGame = await dbContext.BoardGames
            .Include(game => game.PlayerCountRecommendations)
            .SingleOrDefaultAsync(game => game.BggId == 12345);

        Assert.NotNull(persistedGame);
        Assert.Equal("Test Game", persistedGame!.Title);
        Assert.Single(persistedGame.PlayerCountRecommendations);

        var persistedRecommendation = persistedGame.PlayerCountRecommendations.Single();
        Assert.Equal("1", persistedRecommendation.PlayerCount);
        Assert.Equal(20, persistedRecommendation.BestVotes);
        Assert.Equal(15, persistedRecommendation.RecommendedVotes);
        Assert.Equal(5, persistedRecommendation.NotRecommendedVotes);
    }
}
