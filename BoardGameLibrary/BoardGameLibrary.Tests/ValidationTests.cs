using System.Net;
using System.Net.Http.Json;
using System.Text;
using BoardGameLibrary.Api.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

/// <summary>
/// Verifies API-boundary validation and proves invalid requests do not persist state.
/// </summary>
public class ValidationTests
{
    [Fact]
    public async Task Create_WithMissingTitle_ReturnsBadRequestAndDoesNotPersist()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/games", new
        {
            minPlayers = 1,
            maxPlayers = 4,
            minPlayTimeMinutes = 30,
            maxPlayTimeMinutes = 60
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Title", body, StringComparison.OrdinalIgnoreCase);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        Assert.Empty(dbContext.BoardGames);
    }

    [Fact]
    public async Task Create_WithBlankTitle_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/games", ValidBody(title: "   "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithZeroPlayers_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/games", ValidBody(minPlayers: 0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenMinPlayersExceedsMaxPlayers_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            ValidBody(minPlayers: 4, maxPlayers: 2));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task Create_WithZeroPlayTime_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/games", ValidBody(minMinutes: 0, maxMinutes: 60));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenMinPlayTimeExceedsMaxPlayTime_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            ValidBody(minMinutes: 90, maxMinutes: 30));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithTitleOver200Characters_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            ValidBody(title: new string('x', 201)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithInvalidPlayerRating_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/games", new
        {
            title = "Rating Test",
            minPlayers = 1,
            maxPlayers = 4,
            minPlayTimeMinutes = 30,
            maxPlayTimeMinutes = 60,
            playerRatings = new[]
            {
                new { playerCount = 2, rating = 11m }
            }
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithPlayerRatingOutsideSupportedRange_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/games", new
        {
            title = "Rating Range Test",
            minPlayers = 2,
            maxPlayers = 4,
            minPlayTimeMinutes = 30,
            maxPlayTimeMinutes = 60,
            playerRatings = new[]
            {
                new { playerCount = 1, rating = 8m }
            }
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithDuplicatePlayerRatings_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/games", new
        {
            title = "Duplicate Rating Test",
            minPlayers = 1,
            maxPlayers = 4,
            minPlayTimeMinutes = 30,
            maxPlayTimeMinutes = 60,
            playerRatings = new[]
            {
                new { playerCount = 2, rating = 8m },
                new { playerCount = 2, rating = 9m }
            }
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithNullPlayerRatings_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/games", new
        {
            title = "Null Ratings Test",
            minPlayers = 1,
            maxPlayers = 4,
            minPlayTimeMinutes = 30,
            maxPlayTimeMinutes = 60,
            playerRatings = (object?)null
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithNullPlayerRatingItem_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/games", new
        {
            title = "Null Rating Item Test",
            minPlayers = 1,
            maxPlayers = 4,
            minPlayTimeMinutes = 30,
            maxPlayTimeMinutes = 60,
            playerRatings = new object?[] { null }
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithNullJsonBody_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        using var content = new StringContent("null", Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/games", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WithNonPositiveId_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games/0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithInvalidMaxMinutes_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games?maxMinutes=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithRatingSortWithoutPlayers_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games?sort=rating");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task GetAll_WithInvalidSort_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games?sort=unknown");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("abc")]
    public async Task GetAll_WithInvalidPlayerCount_ReturnsBadRequest(string value)
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/games?players={value}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static object ValidBody(
        string title = "Test Game",
        int minPlayers = 1,
        int maxPlayers = 4,
        int minMinutes = 30,
        int maxMinutes = 60) => new
        {
            title,
            minPlayers,
            maxPlayers,
            minPlayTimeMinutes = minMinutes,
            maxPlayTimeMinutes = maxMinutes
        };
}
