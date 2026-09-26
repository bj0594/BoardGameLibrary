using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BoardGameLibrary.Api.Models;

/// <summary>
/// A local library rating for how well a game works at one specific player count.
/// These ratings are intentionally separate from the overall BGG rating.
/// </summary>
public class PlayerCountRating
{
    /// <summary>The owning board game.</summary>
    [JsonIgnore]
    public BoardGame BoardGame { get; set; } = null!;

    /// <summary>Foreign key to the owning board game.</summary>
    public int BoardGameId { get; set; }

    /// <summary>The exact player count this rating describes.</summary>
    public int PlayerCount { get; set; }

    /// <summary>Local rating on a 0-10 scale.</summary>
    public decimal Rating { get; set; }
}
