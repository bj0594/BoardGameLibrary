using System.Net;
using System.Net.Http.Json;
using System.Text;
using BoardGameLibrary.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

/// <summary>Verifies API-level validation rules and their persistence side effects.</summary>
public class ValidationTests
{
    [Fact]
    // Missing required data must be rejected before persistence.
    public async Task Create_WithMissingTitle_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new { minPlayers = 1, maxPlayers = 4 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Title", body, StringComparison.OrdinalIgnoreCase);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        Assert.Empty(dbContext.BoardGames);
    }

    [Theory]
    [InlineData(200, HttpStatusCode.Created)]
    [InlineData(201, HttpStatusCode.BadRequest)]
    // The test covers the exact maximum and first invalid value.
    public async Task Create_TitleLengthBoundary_ReturnsExpectedStatus(
        int titleLength,
        HttpStatusCode expectedStatus)
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new
            {
                title = new string('A', titleLength),
                minPlayers = 1,
                maxPlayers = 4
            });

        Assert.Equal(expectedStatus, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        var persistedCount = await dbContext.BoardGames.CountAsync();

        Assert.Equal(expectedStatus == HttpStatusCode.Created ? 1 : 0, persistedCount);
    }

    [Fact]
    // Whitespace-only input is invalid even though the property is present.
    public async Task Create_WithBlankTitle_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new { title = "   ", minPlayers = 1, maxPlayers = 4 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    // Player counts must be positive.
    public async Task Create_WithZeroMinPlayers_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new { title = "Test", minPlayers = 0, maxPlayers = 4 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    // The upper player-count bound must also be positive.
    public async Task Create_WithZeroMaxPlayers_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new { title = "Test", minPlayers = 1, maxPlayers = 0 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    // The two player-count fields must form a valid inclusive range.
    public async Task Create_WhenMinPlayersExceedsMaxPlayers_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new { title = "Test", minPlayers = 4, maxPlayers = 2 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    // A syntactically valid JSON request containing null is still an invalid resource body.
    public async Task Create_WithNullJsonBody_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        using var content = new StringContent("null", Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/games", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    // Invalid resource identifiers are rejected at the HTTP boundary.
    public async Task GetById_WithNonPositiveId_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games/0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        Assert.Empty(dbContext.BoardGames);
    }
}
