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

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new CreateBoardGameRequest { BggId = 12345 });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/games/12345", response.Headers.Location?.AbsolutePath);
        Assert.Equal(1, factory.FakeBggClient.CallCount);

        var createdGame = await response.Content.ReadFromJsonAsync<BoardGame>();

        Assert.NotNull(createdGame);
        Assert.Equal(12345, createdGame!.BggId);
        Assert.Equal("Test Game", createdGame.Title);
        Assert.Equal(1, createdGame.MinPlayers);
        Assert.Equal(4, createdGame.MaxPlayers);
        Assert.Equal(8.2, createdGame.BggAverageRating);
        Assert.Equal(1000, createdGame.BggRatingCount);
        Assert.Equal(3, createdGame.PlayerCountRecommendations.Count);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

        var persistedGame = await dbContext.BoardGames
            .Include(game => game.PlayerCountRecommendations)
            .SingleOrDefaultAsync(game => game.BggId == 12345);

        Assert.NotNull(persistedGame);
        Assert.Equal("Test Game", persistedGame!.Title);
        Assert.Equal(3, persistedGame.PlayerCountRecommendations.Count);

        var soloRecommendation = persistedGame.PlayerCountRecommendations
            .Single(recommendation => recommendation.PlayerCount == "1");

        Assert.Equal(20, soloRecommendation.BestVotes);
        Assert.Equal(15, soloRecommendation.RecommendedVotes);
        Assert.Equal(5, soloRecommendation.NotRecommendedVotes);

        Assert.Contains(
            persistedGame.PlayerCountRecommendations,
            recommendation => recommendation.PlayerCount == "4+");
    }
}
