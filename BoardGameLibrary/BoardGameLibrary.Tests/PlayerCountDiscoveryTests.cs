using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

/// <summary>
/// Verifies the portfolio feature: find and rank games for a concrete player count.
/// </summary>
public class PlayerCountDiscoveryTests
{
    [Fact]
    public async Task GetByPlayerCount_ForOnePlayer_ReturnsOnlyCompatibleGamesAndSelectedRating()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, Game("Solo Game", 1, 4, 30, 60,
            (1, 8.2m), (2, 8.0m), (3, 8.0m), (4, 7.8m)));
        await SeedAsync(factory, Game("Two Player Game", 2, 4, 30, 60,
            (2, 9.0m), (3, 8.0m), (4, 8.0m)));

        var response = await client.GetAsync("/api/games?players=1");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        var game = Assert.Single(games!);
        Assert.Equal("Solo Game", game.Title);
        Assert.Equal(8.2m, game.SelectedPlayerRating);
    }

    [Fact]
    public async Task GetByPlayerCount_ForSameGame_UsesRatingForSelectedPlayerCount()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var gameId = await SeedAsync(factory, Game("Variable Rating Game", 1, 4, 30, 60,
            (1, 7.0m), (2, 8.0m), (3, 9.0m), (4, 7.5m)));

        var twoPlayerResponse = await client.GetAsync($"/api/games/{gameId}?players=2");
        var twoPlayerGame = await twoPlayerResponse.Content.ReadFromJsonAsync<BoardGame>();

        var fourPlayerResponse = await client.GetAsync($"/api/games/{gameId}?players=4");
        var fourPlayerGame = await fourPlayerResponse.Content.ReadFromJsonAsync<BoardGame>();

        Assert.Equal(HttpStatusCode.OK, twoPlayerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, fourPlayerResponse.StatusCode);
        Assert.Equal(2, twoPlayerGame!.SelectedPlayerCount);
        Assert.Equal(8.0m, twoPlayerGame.SelectedPlayerRating);
        Assert.Equal(4, fourPlayerGame!.SelectedPlayerCount);
        Assert.Equal(7.5m, fourPlayerGame.SelectedPlayerRating);
    }

    [Fact]
    public async Task GetByPlayerCount_AtMaxPlayers_IncludesBoundaryMatch()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, Game("Four Player Game", 1, 4, 30, 60,
            (1, 7.0m), (2, 7.5m), (3, 8.0m), (4, 8.8m)));

        var response = await client.GetAsync("/api/games?players=4");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        var game = Assert.Single(games!);
        Assert.Equal(8.8m, game.SelectedPlayerRating);
    }

    [Fact]
    public async Task GetByPlayerCount_WithRatingSort_ReturnsHighestRatingFirst()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, Game("Lower Rated", 2, 4, 30, 60,
            (2, 8.0m), (3, 8.1m), (4, 7.8m)));
        await SeedAsync(factory, Game("Higher Rated", 2, 4, 30, 60,
            (2, 8.2m), (3, 8.9m), (4, 8.6m)));

        var response = await client.GetAsync("/api/games?players=3&sort=rating");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        Assert.Equal(["Higher Rated", "Lower Rated"], games!.Select(game => game.Title));
        Assert.Equal(8.9m, games[0].SelectedPlayerRating);
    }

    [Fact]
    public async Task GetByPlayerCount_WithWhitespaceRatingSort_NormalizesInput()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, Game("Lower Rated", 2, 4, 30, 60,
            (2, 8.0m), (3, 8.1m), (4, 7.8m)));
        await SeedAsync(factory, Game("Higher Rated", 2, 4, 30, 60,
            (2, 8.2m), (3, 8.9m), (4, 8.6m)));

        var response = await client.GetAsync("/api/games?players=3&sort=%20rating%20");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        Assert.Equal("Higher Rated", games![0].Title);
        Assert.Equal(8.9m, games[0].SelectedPlayerRating);
    }

    [Fact]
    public async Task GetByPlayerCount_WithMaxMinutes_ExcludesGamesThatTakeTooLong()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, Game("Short Game", 2, 4, 30, 60,
            (2, 8.0m), (3, 8.0m), (4, 8.0m)));
        await SeedAsync(factory, Game("Long Game", 2, 4, 60, 120,
            (2, 9.0m), (3, 9.0m), (4, 9.0m)));

        var response = await client.GetAsync("/api/games?players=4&maxMinutes=60");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        var game = Assert.Single(games!);
        Assert.Equal("Short Game", game.Title);
    }

    [Fact]
    public async Task GetByPlayerCount_WithRatingAndTimeFilters_ReturnsBestMatchingGame()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, Game("Fast Lower Rated", 2, 4, 30, 60,
            (2, 8.0m), (3, 8.1m), (4, 8.0m)));
        await SeedAsync(factory, Game("Slow Higher Rated", 2, 4, 60, 120,
            (2, 9.0m), (3, 9.1m), (4, 9.4m)));
        await SeedAsync(factory, Game("Fast Higher Rated", 2, 4, 45, 90,
            (2, 8.8m), (3, 9.0m), (4, 9.2m)));

        var response = await client.GetAsync(
            "/api/games?players=4&maxMinutes=90&sort=rating");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        Assert.Equal(["Fast Higher Rated", "Fast Lower Rated"], games!.Select(game => game.Title));
    }


    [Fact]
    public async Task GetAll_WithPlaytimeSort_ReturnsShortestGamesFirst()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, Game("Long Game", 2, 4, 60, 120,
            (2, 8.0m), (3, 8.0m), (4, 8.0m)));
        await SeedAsync(factory, Game("Short Game", 2, 4, 30, 45,
            (2, 8.0m), (3, 8.0m), (4, 8.0m)));

        var response = await client.GetAsync("/api/games?sort=playtime");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        Assert.Equal(["Short Game", "Long Game"], games!.Select(game => game.Title));
    }

    [Fact]
    public async Task GetByPlayerCount_WithNoMatches_ReturnsEmptyCollection()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        await SeedAsync(factory, Game("Two Player Game", 1, 2, 30, 60,
            (1, 8.0m), (2, 8.0m)));

        var response = await client.GetAsync("/api/games?players=4");
        var games = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(games);
        Assert.Empty(games!);
    }

    private static BoardGame Game(
        string title,
        int minPlayers,
        int maxPlayers,
        int minMinutes,
        int maxMinutes,
        params (int PlayerCount, decimal Rating)[] ratings) =>
        new()
        {
            Title = title,
            MinPlayers = minPlayers,
            MaxPlayers = maxPlayers,
            MinPlayTimeMinutes = minMinutes,
            MaxPlayTimeMinutes = maxMinutes,
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = ratings.Select(rating => new PlayerCountRating
            {
                PlayerCount = rating.PlayerCount,
                Rating = rating.Rating
            }).ToList()
        };

    private static async Task<int> SeedAsync(BoardGameApiFactory factory, BoardGame game)
	{
    		using var scope = factory.Services.CreateScope();
    		var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
    		dbContext.BoardGames.Add(game);
    		await dbContext.SaveChangesAsync();

    		return game.Id;
	}
}
