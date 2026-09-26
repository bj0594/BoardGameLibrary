using System.Net;

namespace BoardGameLibrary.Tests;

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

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new
            {
                title = "Test Game",
                minPlayers = 1,
                maxPlayers = 4
            });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
