using System.ComponentModel.DataAnnotations;

namespace BoardGameLibrary.Api.Models;

/// <summary>
/// Request-side representation of one player-count-specific library rating.
/// </summary>
public class PlayerCountRatingInput
{
    /// <summary>The player count the rating applies to.</summary>
    [Range(1, int.MaxValue)]
    public int PlayerCount { get; set; }

    /// <summary>Local rating from 0 to 10.</summary>
    [Range(typeof(decimal), "0", "10")]
    public decimal Rating { get; set; }
}
