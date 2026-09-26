using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

public class ValidationTests
{
    [Fact]
    public async Task CreateWithNonPositiveBggId_ReturnsBadRequestAndDoesNotCallBgg()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new CreateBoardGameRequest { BggId = 0 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, factory.FakeBggClient.CallCount);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

        Assert.Empty(dbContext.BoardGames);
    }

    [Fact]
    public async Task GetByIdWithNonPositiveBggId_ReturnsBadRequest()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/games/0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
