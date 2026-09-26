using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameLibrary.Api.SeedData;

/// <summary>
/// Adds a small demonstration library the first time a Development database is empty.
/// BGG fields are snapshot metadata; PlayerCountRating values are local demo ratings.
/// </summary>
public static class BoardGameSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BoardGameDbContext>();

        if (await dbContext.BoardGames.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.BoardGames.AddRange(CreateDemoGames());
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static List<BoardGame> CreateDemoGames() =>
    [
        // BGG snapshot: https://boardgamegeek.com/boardgame/295947/cascadia
        new BoardGame
        {
            BggId = 295947,
            Title = "Cascadia",
            MinPlayers = 1,
            MaxPlayers = 4,
            MinPlayTimeMinutes = 30,
            MaxPlayTimeMinutes = 45,
            BggAverageRating = 7.882m,
            BggBestWith = "2-3",
            BggUrl = "https://boardgamegeek.com/boardgame/295947/cascadia",
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = Ratings((1, 8.0m), (2, 9.2m), (3, 9.1m), (4, 8.3m))
        },
        // BGG snapshot: https://boardgamegeek.com/boardgame/266192/wingspan
        new BoardGame
        {
            BggId = 266192,
            Title = "Wingspan",
            MinPlayers = 1,
            MaxPlayers = 5,
            MinPlayTimeMinutes = 40,
            MaxPlayTimeMinutes = 70,
            BggAverageRating = 7.994m,
            BggBestWith = "3",
            BggUrl = "https://boardgamegeek.com/boardgame/266192/wingspan",
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = Ratings((1, 8.2m), (2, 8.6m), (3, 9.2m), (4, 8.8m), (5, 8.1m))
        },
        // BGG snapshot: https://boardgamegeek.com/boardgame/316554/dune-imperium
        new BoardGame
        {
            BggId = 316554,
            Title = "Dune: Imperium",
            MinPlayers = 1,
            MaxPlayers = 4,
            MinPlayTimeMinutes = 60,
            MaxPlayTimeMinutes = 120,
            BggAverageRating = 8.41m,
            BggBestWith = "3-4",
            BggUrl = "https://boardgamegeek.com/boardgame/316554/dune-imperium",
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = Ratings((1, 8.1m), (2, 8.6m), (3, 9.2m), (4, 9.1m))
        },
        // BGG snapshot: https://boardgamegeek.com/boardgame/230802/azul
        new BoardGame
        {
            BggId = 230802,
            Title = "Azul",
            MinPlayers = 2,
            MaxPlayers = 4,
            MinPlayTimeMinutes = 30,
            MaxPlayTimeMinutes = 45,
            BggAverageRating = 7.71m,
            BggBestWith = "2",
            BggUrl = "https://boardgamegeek.com/boardgame/230802/azul",
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = Ratings((2, 8.9m), (3, 8.4m), (4, 8.0m))
        },
        // BGG snapshot: https://boardgamegeek.com/boardgame/822/carcassonne
        new BoardGame
        {
            BggId = 822,
            Title = "Carcassonne",
            MinPlayers = 2,
            MaxPlayers = 5,
            MinPlayTimeMinutes = 30,
            MaxPlayTimeMinutes = 45,
            BggAverageRating = 7.4m,
            BggBestWith = "2",
            BggUrl = "https://boardgamegeek.com/boardgame/822/carcassonne",
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = Ratings((2, 8.8m), (3, 8.3m), (4, 7.9m), (5, 7.5m))
        },
        // BGG snapshot: https://boardgamegeek.com/boardgame/148228/splendor
        new BoardGame
        {
            BggId = 148228,
            Title = "Splendor",
            MinPlayers = 2,
            MaxPlayers = 4,
            MinPlayTimeMinutes = 30,
            MaxPlayTimeMinutes = 30,
            BggAverageRating = 7.421m,
            BggBestWith = "3",
            BggUrl = "https://boardgamegeek.com/boardgame/148228/splendor",
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = Ratings((2, 8.0m), (3, 8.7m), (4, 8.2m))
        },
        // BGG snapshot: https://boardgamegeek.com/boardgame/9209/ticket-to-ride
        new BoardGame
        {
            BggId = 9209,
            Title = "Ticket to Ride",
            MinPlayers = 2,
            MaxPlayers = 5,
            MinPlayTimeMinutes = 30,
            MaxPlayTimeMinutes = 60,
            BggAverageRating = 7.4m,
            BggBestWith = "4",
            BggUrl = "https://boardgamegeek.com/boardgame/9209/ticket-to-ride",
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = Ratings((2, 7.6m), (3, 8.1m), (4, 8.6m), (5, 8.2m))
        },
        // BGG snapshot: https://boardgamegeek.com/boardgame/167791/terraforming-mars
        new BoardGame
        {
            BggId = 167791,
            Title = "Terraforming Mars",
            MinPlayers = 1,
            MaxPlayers = 5,
            MinPlayTimeMinutes = 120,
            MaxPlayTimeMinutes = 120,
            BggAverageRating = 8.331m,
            BggBestWith = "3",
            BggUrl = "https://boardgamegeek.com/boardgame/167791/terraforming-mars",
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = Ratings((1, 8.0m), (2, 8.4m), (3, 8.9m), (4, 8.5m), (5, 7.8m))
        },
        // BGG snapshot: https://boardgamegeek.com/boardgame/342942/ark-nova
        new BoardGame
        {
            BggId = 342942,
            Title = "Ark Nova",
            MinPlayers = 1,
            MaxPlayers = 4,
            MinPlayTimeMinutes = 90,
            MaxPlayTimeMinutes = 150,
            BggAverageRating = 8.538m,
            BggBestWith = "2",
            BggUrl = "https://boardgamegeek.com/boardgame/342942/ark-nova",
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = Ratings((1, 8.5m), (2, 9.1m), (3, 8.4m), (4, 7.9m))
        },
        // BGG snapshot: https://boardgamegeek.com/boardgame/30549/pandemic
        new BoardGame
        {
            BggId = 30549,
            Title = "Pandemic",
            MinPlayers = 2,
            MaxPlayers = 4,
            MinPlayTimeMinutes = 45,
            MaxPlayTimeMinutes = 45,
            BggAverageRating = 7.5m,
            BggBestWith = "4",
            BggUrl = "https://boardgamegeek.com/boardgame/30549/pandemic",
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = Ratings((2, 7.9m), (3, 8.4m), (4, 8.9m))
        }
    ];

    private static List<PlayerCountRating> Ratings(params (int PlayerCount, decimal Rating)[] ratings) =>
        ratings
            .Select(rating => new PlayerCountRating
            {
                PlayerCount = rating.PlayerCount,
                Rating = rating.Rating
            })
            .ToList();
}
