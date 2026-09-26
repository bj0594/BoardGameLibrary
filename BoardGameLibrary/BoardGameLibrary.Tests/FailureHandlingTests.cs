using System.Net;
using System.Net.Http.Json;

namespace BoardGameLibrary.Tests;

/// <summary>Verifies that database failures become stable API-level 500 responses.</summary>
public class FailureHandlingTests
{
    [Fact]
    // Breaking the test database before GET simulates a persistence-layer failure.
    public async Task Get_WhenDatabaseFails_ReturnsInternalServerError()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        factory.BreakDatabaseConnection();

        var response = await client.GetAsync("/api/games");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    // POST must also translate persistence failure into a stable 500 response without leaking database details.
    public async Task Create_WhenDatabaseFails_ReturnsInternalServerError()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        factory.BreakDatabaseConnection();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new
            {
                title = "Test Game",
                minPlayers = 1,
                maxPlayers = 4
            });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("SqliteException", body, StringComparison.OrdinalIgnoreCase);
    }
}
