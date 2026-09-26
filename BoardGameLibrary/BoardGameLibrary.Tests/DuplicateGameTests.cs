using System.Net;
using BoardGameLibrary.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameLibrary.Tests;

public class DuplicateGameTests
{
    [Fact]
    public async Task CreatingSameGameTwice_ReturnsConflictAndPersistsOnlyOneGame()
    {
        using var factory = new BoardGameApiFactory();
        using var client = factory.CreateClient();

        var firstResponse = await client.PostAsJsonAsync(
            "/api/games",
            new { BggId = 777 });
        var secondResponse = await client.PostAsJsonAsync(
            "/api/games",
            new { BggId = 777 });

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        Assert.Equal(1, factory.FakeBggClient.CallCount);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

        Assert.Equal(1, await dbContext.BoardGames.CountAsync());
        Assert.Equal(3, await dbContext.PlayerCountRecommendations.CountAsync());
    }
}
