using System.Net;
using BoardGameLibrary.Api.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

public class FailureHandlingTests
{
    [Fact]
    public async Task Create_WhenBggRequestFails_ReturnsBadGatewayAndDoesNotPersist()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();
        factory.FakeBggClient.FailureMode = FakeBggFailureMode.HttpRequestException;

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new { BggId = 54321 });

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        Assert.Empty(dbContext.BoardGames);
    }

    [Fact]
    public async Task Create_WhenBggGameDoesNotExist_ReturnsNotFoundAndDoesNotPersist()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();
        factory.FakeBggClient.FailureMode = FakeBggFailureMode.NotFound;

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new { BggId = 54321 });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        Assert.Empty(dbContext.BoardGames);
    }

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
            new { BggId = 12345 });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
