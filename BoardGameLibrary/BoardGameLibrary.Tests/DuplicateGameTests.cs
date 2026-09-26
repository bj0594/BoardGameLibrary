using System.Net;
using System.Net.Http.Json;
using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
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

        var request = new CreateBoardGameRequest
        {
            BggId = 777
        };

        var firstResponse = await client.PostAsJsonAsync("/api/games", request);
        var secondResponse = await client.PostAsJsonAsync("/api/games", request);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
        Assert.Equal(1, factory.FakeBggClient.CallCount);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

        Assert.Equal(1, await dbContext.BoardGames.CountAsync());
        Assert.Equal(1, await dbContext.PlayerCountRecommendations.CountAsync());
    }
}
