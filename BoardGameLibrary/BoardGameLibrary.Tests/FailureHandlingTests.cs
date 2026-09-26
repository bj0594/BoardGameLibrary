using System.Net;
using System.Net.Http.Json;

namespace BoardGameLibrary.Tests;

/// <summary>
/// Confirms that infrastructure failures become safe HTTP 500 responses.
/// </summary>
public class FailureHandlingTests
{
    [Fact]
    public async Task Get_WhenDatabaseFails_ReturnsInternalServerError()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        factory.BreakDatabaseConnection();

        var response = await client.GetAsync("/api/games");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenDatabaseFails_ReturnsInternalServerError()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        factory.BreakDatabaseConnection();

        var response = await client.PostAsJsonAsync("/api/games", new
        {
            title = "Test Game",
            minPlayers = 1,
            maxPlayers = 4,
            minPlayTimeMinutes = 30,
            maxPlayTimeMinutes = 60
        });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
