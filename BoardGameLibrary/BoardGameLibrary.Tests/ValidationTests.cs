using System.Net;
using BoardGameLibrary.Api.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

public class ValidationTests
{
    [Fact]
    public async Task CreateWithZeroBggId_ReturnsBadRequestAndDoesNotCallBgg()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new { BggId = 0 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, factory.FakeBggClient.CallCount);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        Assert.Empty(dbContext.BoardGames);
    }

    [Fact]
    public async Task CreateWithNegativeBggId_ReturnsBadRequestAndDoesNotCallBgg()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new { BggId = -1 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, factory.FakeBggClient.CallCount);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();
        Assert.Empty(dbContext.BoardGames);
    }

    [Fact]
    public async Task CreateWithMissingRequestBody_ReturnsBadRequestAndDoesNotCallBgg()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/games");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, factory.FakeBggClient.CallCount);
    }

    [Fact]
    public async Task GetByIdWithZero_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games/0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetByIdWithNegativeId_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games/-1");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
