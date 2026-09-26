using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BoardGameLibrary.Api.Models;

/// <summary>
/// Persistent representation of a board game in the local library.
/// BGG fields are snapshot metadata; player-count ratings are local library data.
/// </summary>
public class BoardGame
{
    /// <summary>Local database identity.</summary>
    public int Id { get; set; }

    /// <summary>Optional BoardGameGeek identifier for the imported snapshot.</summary>
    public int? BggId { get; set; }

    /// <summary>The game's display title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Lowest supported player count according to the stored source data.</summary>
    public int MinPlayers { get; set; }

    /// <summary>Highest supported player count according to the stored source data.</summary>
    public int MaxPlayers { get; set; }

    /// <summary>Minimum play time in minutes from the stored source data.</summary>
    public int MinPlayTimeMinutes { get; set; }

    /// <summary>Maximum play time in minutes from the stored source data.</summary>
    public int MaxPlayTimeMinutes { get; set; }

    /// <summary>Snapshot of the public BGG average rating at import time.</summary>
    public decimal? BggAverageRating { get; set; }

    /// <summary>Snapshot of BGG's community best-with player-count information.</summary>
    public string? BggBestWith { get; set; }

    /// <summary>Source URL used for the stored BGG snapshot.</summary>
    public string? BggUrl { get; set; }

    /// <summary>UTC time at which the local resource was created.</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Local rating for each supported player count.</summary>
    public List<PlayerCountRating> PlayerCountRatings { get; set; } = [];

    /// <summary>
    /// The rating selected by the current GET players query. This is not persisted.
    /// </summary>
    [NotMapped]
    public decimal? SelectedPlayerRating { get; set; }

    /// <summary>The player count associated with <see cref="SelectedPlayerRating"/>.</summary>
    [NotMapped]
    public int? SelectedPlayerCount { get; set; }
}
