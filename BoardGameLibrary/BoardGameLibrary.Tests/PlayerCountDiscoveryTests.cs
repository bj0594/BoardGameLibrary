using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

public class PlayerCountDiscoveryTests
{
    [Fact]
    public async Task GetByPlayerCount_ReturnsOnlyGamesWithMatchingRecommendation()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

            dbContext.BoardGames.AddRange(
                new BoardGame
                {
                    BggId = 100,
                    Title = "Solo Candidate",
                    MinPlayers = 1,
                    MaxPlayers = 4,
                    PlayerCountRecommendations =
                    [
                        new PlayerCountRecommendation
                        {
                            PlayerCount = "1",
                            BestVotes = 20,
                            RecommendedVotes = 10,
                            NotRecommendedVotes = 2
                        },
                        new PlayerCountRecommendation
                        {
                            PlayerCount = "2",
                            BestVotes = 30,
                            RecommendedVotes = 20,
                            NotRecommendedVotes = 1
                        }
                    ]
                },
                new BoardGame
                {
                    BggId = 200,
                    Title = "Two Player Game",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    PlayerCountRecommendations =
                    [
                        new PlayerCountRecommendation
                        {
                            PlayerCount = "2",
                            BestVotes = 40,
                            RecommendedVotes = 10,
                            NotRecommendedVotes = 1
                        }
                    ]
                });

            await dbContext.SaveChangesAsync();
        }

        var response = await client.GetAsync("/api/games?players=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.NotNull(games);
        var game = Assert.Single(games!);
        Assert.Equal(100, game.BggId);
        Assert.Equal("Solo Candidate", game.Title);
        Assert.Contains(game.PlayerCountRecommendations,
            recommendation => recommendation.PlayerCount == "1");
    }

    [Fact]
    public async Task GetByPlayerCount_WithZero_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games?players=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
