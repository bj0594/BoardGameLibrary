using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

public class PlayerCountDiscoveryTests
{
    [Fact]
    public async Task GetByPlayerCount_ForOnePlayer_ReturnsOnlyGamesWithMatchingRecommendation()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedGamesAsync(factory);

        var response = await client.GetAsync("/api/games?players=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.NotNull(games);
        var game = Assert.Single(games!);
        Assert.Equal(100, game.BggId);
        Assert.Contains(game.PlayerCountRecommendations,
            recommendation => recommendation.PlayerCount == "1");
    }

    [Fact]
    public async Task GetByPlayerCount_ForTwoPlayers_ReturnsOnlyGamesWithMatchingRecommendation()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedGamesAsync(factory);

        var response = await client.GetAsync("/api/games?players=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.NotNull(games);
        Assert.Equal(2, games!.Count);
        Assert.All(games, game =>
            Assert.Contains(game.PlayerCountRecommendations,
                recommendation => recommendation.PlayerCount == "2"));
    }

    [Fact]
    public async Task GetByPlayerCount_ForFourPlusPlayers_SupportsBggBucket()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedGamesAsync(factory);

        var response = await client.GetAsync("/api/games?players=4%2B");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.NotNull(games);
        var game = Assert.Single(games!);
        Assert.Equal(100, game.BggId);
        Assert.Contains(game.PlayerCountRecommendations,
            recommendation => recommendation.PlayerCount == "4+");
    }

    [Fact]
    public async Task GetByPlayerCount_WhenNoGameMatches_ReturnsEmptyCollection()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedGamesAsync(factory);

        var response = await client.GetAsync("/api/games?players=3");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.NotNull(games);
        Assert.Empty(games!);
    }

    [Fact]
    public async Task GetByPlayerCount_WithZero_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games?players=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetByPlayerCount_WithNonNumericValue_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games?players=abc");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task SeedGamesAsync(BoardGameApiFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

        dbContext.BoardGames.AddRange(
            new BoardGame
            {
                BggId = 100,
                Title = "Player Count Game",
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
                    },
                    new PlayerCountRecommendation
                    {
                        PlayerCount = "4+",
                        BestVotes = 12,
                        RecommendedVotes = 8,
                        NotRecommendedVotes = 3
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
}
