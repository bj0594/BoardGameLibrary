using System.Net;
using System.Text;
using BoardGameLibrary.Api.Services;
using Microsoft.Extensions.Configuration;

namespace BoardGameLibrary.Tests;

public class BoardGameGeekClientTests
{
    [Fact]
    public async Task GetBoardGame_MapsCoreDataAndPlayerCountPoll()
    {
        const string xml = """
        <?xml version="1.0" encoding="UTF-8"?>
        <items totalitems="1">
          <item type="boardgame" id="12345">
            <name type="primary" sortindex="1" value="Test Game" />
            <minplayers value="1" />
            <maxplayers value="4" />
            <statistics>
              <ratings>
                <usersrated value="1000" />
                <average value="8.2" />
              </ratings>
            </statistics>
            <poll name="suggested_numplayers">
              <results numplayers="1">
                <result value="Best" numvotes="20" />
                <result value="Recommended" numvotes="15" />
                <result value="Not Recommended" numvotes="5" />
              </results>
              <results numplayers="4+">
                <result value="Best" numvotes="12" />
                <result value="Recommended" numvotes="8" />
                <result value="Not Recommended" numvotes="3" />
              </results>
            </poll>
          </item>
        </items>
        """;

        var handler = new StubHandler(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            });

        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/")
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BoardGameGeek:AuthorizationToken"] = "test-token"
            })
            .Build();

        var sut = new BoardGameGeekClient(client, configuration);

        var game = await sut.GetBoardGameAsync(12345);

        Assert.NotNull(game);
        Assert.Equal(
            "/xmlapi2/thing?id=12345&stats=1",
            handler.LastRequestPathAndQuery);
        Assert.Equal(
            "Bearer",
            handler.LastAuthorizationScheme);
        Assert.Equal(
            "test-token",
            handler.LastAuthorizationParameter);
        Assert.Equal(12345, game!.BggId);
        Assert.Equal("Test Game", game.Title);
        Assert.Equal(1, game.MinPlayers);
        Assert.Equal(4, game.MaxPlayers);
        Assert.Equal(8.2, game.BggAverageRating);
        Assert.Equal(1000, game.BggRatingCount);

        var solo = Assert.Single(game.PlayerCountRecommendations,
            recommendation => recommendation.PlayerCount == "1");
        Assert.Equal(20, solo.BestVotes);
        Assert.Equal(15, solo.RecommendedVotes);
        Assert.Equal(5, solo.NotRecommendedVotes);

        var fourPlus = Assert.Single(game.PlayerCountRecommendations,
            recommendation => recommendation.PlayerCount == "4+");
        Assert.Equal(12, fourPlus.BestVotes);
        Assert.Equal(8, fourPlus.RecommendedVotes);
        Assert.Equal(3, fourPlus.NotRecommendedVotes);
    }

    [Fact]
    public async Task GetBoardGame_WhenBggReturnsNotFound_ReturnsNull()
    {
        using var client = new HttpClient(new StubHandler(
            new HttpResponseMessage(HttpStatusCode.NotFound)))
        {
            BaseAddress = new Uri("https://example.test/")
        };

        var configuration = new ConfigurationBuilder().Build();
        var sut = new BoardGameGeekClient(client, configuration);

        var game = await sut.GetBoardGameAsync(12345);

        Assert.Null(game);
    }

    [Fact]
    public async Task GetBoardGame_WhenBggReturnsFailure_ThrowsHttpRequestException()
    {
        using var client = new HttpClient(new StubHandler(
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)))
        {
            BaseAddress = new Uri("https://example.test/")
        };

        var configuration = new ConfigurationBuilder().Build();
        var sut = new BoardGameGeekClient(client, configuration);

        await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetBoardGameAsync(12345));
    }

    private sealed class StubHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public string? LastRequestPathAndQuery { get; private set; }
        public string? LastAuthorizationScheme { get; private set; }
        public string? LastAuthorizationParameter { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequestPathAndQuery = request.RequestUri?.PathAndQuery;
            LastAuthorizationScheme = request.Headers.Authorization?.Scheme;
            LastAuthorizationParameter = request.Headers.Authorization?.Parameter;

            return Task.FromResult(new HttpResponseMessage(response.StatusCode)
            {
                Content = response.Content,
                RequestMessage = request
            });
        }
    }
}
