using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Linq;
using BoardGameLibrary.Api.Models;

namespace BoardGameLibrary.Api.Services;

public class BoardGameGeekClient(
    HttpClient httpClient,
    IConfiguration configuration) : IBoardGameGeekClient
{
    public async Task<BoardGame?> GetBoardGameAsync(
        int bggId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"xmlapi2/thing?id={bggId}&stats=1");

        var token = configuration["BoardGameGeek:AuthorizationToken"];

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"BoardGameGeek returned HTTP {(int)response.StatusCode}.");
        }

        var xml = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            return ParseBoardGame(xml, bggId);
        }
        catch (XmlException exception)
        {
            throw new HttpRequestException(
                "BoardGameGeek returned malformed XML.",
                exception);
        }
    }

    private static BoardGame? ParseBoardGame(
        string xml,
        int requestedBggId)
    {
        var document = XDocument.Parse(xml);
        var item = document.Root?.Element("item");

        if (item is null)
        {
            return null;
        }

        var title = item.Elements("name")
            .FirstOrDefault(name =>
                (string?)name.Attribute("type") == "primary")?
            .Attribute("value")?
            .Value;

        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        var game = new BoardGame
        {
            BggId = (int?)item.Attribute("id") ?? requestedBggId,
            Title = title,
            MinPlayers = GetIntAttribute(
                item.Element("minplayers"),
                "value"),
            MaxPlayers = GetIntAttribute(
                item.Element("maxplayers"),
                "value")
        };

        var ratings = item.Element("statistics")?.Element("ratings");

        game.BggAverageRating = GetDoubleAttribute(
            ratings?.Element("average"),
            "value");

        game.BggRatingCount = GetNullableIntAttribute(
            ratings?.Element("usersrated"),
            "value");

        var playerPoll = item.Elements("poll")
            .FirstOrDefault(poll =>
                (string?)poll.Attribute("name") ==
                "suggested_numplayers");

        if (playerPoll is null)
        {
            return game;
        }

        foreach (var results in playerPoll.Elements("results"))
        {
            var playerCountValue =
                (string?)results.Attribute("numplayers");

            if (string.IsNullOrWhiteSpace(playerCountValue))
            {
                continue;
            }

            game.PlayerCountRecommendations.Add(
                new PlayerCountRecommendation
                {
                    PlayerCount = playerCountValue,
                    BestVotes = GetPollVotes(results, "Best"),
                    RecommendedVotes =
                        GetPollVotes(results, "Recommended"),
                    NotRecommendedVotes =
                        GetPollVotes(results, "Not Recommended")
                });
        }

        return game;
    }

    private static int GetPollVotes(
        XElement results,
        string value)
    {
        var result = results.Elements("result")
            .FirstOrDefault(element =>
                (string?)element.Attribute("value") == value);

        return GetIntAttribute(result, "numvotes");
    }

    private static int GetIntAttribute(
        XElement? element,
        string attributeName)
        => int.TryParse(
            (string?)element?.Attribute(attributeName),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var value)
            ? value
            : 0;

    private static int? GetNullableIntAttribute(
        XElement? element,
        string attributeName)
        => int.TryParse(
            (string?)element?.Attribute(attributeName),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var value)
            ? value
            : null;

    private static double? GetDoubleAttribute(
        XElement? element,
        string attributeName)
        => double.TryParse(
            (string?)element?.Attribute(attributeName),
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out var value)
            ? value
            : null;
}
