using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

public class GetBoardGamesTests
{
    [Fact]
    public async Task GetAll_ReturnsPersistedGamesInDefinedOrder()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

            dbContext.BoardGames.AddRange(
                new BoardGame
                {
                    BggId = 200,
                    Title = "Zeta Game",
                    MinPlayers = 1,
                    MaxPlayers = 4,
                    BggAverageRating = 7.8,
                    BggRatingCount = 300
                },
                new BoardGame
                {
                    BggId = 100,
                    Title = "Alpha Game",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    BggAverageRating = 8.1,
                    BggRatingCount = 500
                });

            await dbContext.SaveChangesAsync();
        }

        var response = await client.GetAsync("/api/games");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.NotNull(games);
        Assert.Equal(2, games!.Count);
        Assert.Equal("Alpha Game", games[0].Title);
        Assert.Equal("Zeta Game", games[1].Title);
    }

    [Fact]
    public async Task GetById_ReturnsStoredGameWithRecommendations()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

            dbContext.BoardGames.Add(new BoardGame
            {
                BggId = 321,
                Title = "Stored Game",
                MinPlayers = 1,
                MaxPlayers = 4,
                PlayerCountRecommendations =
                [
                    new PlayerCountRecommendation
                    {
                        PlayerCount = "1",
                        BestVotes = 12,
                        RecommendedVotes = 8,
                        NotRecommendedVotes = 2
                    }
                ]
            });

            await dbContext.SaveChangesAsync();
        }

        var response = await client.GetAsync("/api/games/321");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var game = await response.Content.ReadFromJsonAsync<BoardGame>();

        Assert.NotNull(game);
        Assert.Equal(321, game!.BggId);
        Assert.Equal("Stored Game", game.Title);
        Assert.Single(game.PlayerCountRecommendations);
        Assert.Equal("1", game.PlayerCountRecommendations[0].PlayerCount);
    }

    [Fact]
    public async Task GetById_WhenGameDoesNotExist_ReturnsNotFound()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
