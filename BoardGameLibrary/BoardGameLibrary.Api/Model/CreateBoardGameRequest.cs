using System.ComponentModel.DataAnnotations;

namespace BoardGameLibrary.Api.Models;

/// <summary>
/// API input required to add a local board game to the library.
/// </summary>
public class CreateBoardGameRequest : IValidatableObject
{
    /// <summary>Game title entered by the library owner.</summary>
    [Required]
    [StringLength(200)]
    public string? Title { get; set; }

    /// <summary>Lowest supported player count.</summary>
    [Range(1, int.MaxValue)]
    public int MinPlayers { get; set; }

    /// <summary>Highest supported player count.</summary>
    [Range(1, int.MaxValue)]
    public int MaxPlayers { get; set; }

    /// <summary>Shortest expected play time in minutes.</summary>
    [Range(1, 1000)]
    public int MinPlayTimeMinutes { get; set; }

    /// <summary>Longest expected play time in minutes.</summary>
    [Range(1, 1000)]
    public int MaxPlayTimeMinutes { get; set; }

    /// <summary>Optional ratings that make discovery more useful for each player count.</summary>
    public List<PlayerCountRatingInput> PlayerRatings { get; set; } = [];

    /// <summary>
    /// Cross-field validation that cannot be expressed cleanly with single-property attributes.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            yield return new ValidationResult(
                "Title is required and cannot be blank.",
                [nameof(Title)]);
        }

        if (MinPlayers > 0 && MaxPlayers > 0 && MinPlayers > MaxPlayers)
        {
            yield return new ValidationResult(
                "MinPlayers cannot be greater than MaxPlayers.",
                [nameof(MinPlayers), nameof(MaxPlayers)]);
        }

        if (MinPlayTimeMinutes > 0 && MaxPlayTimeMinutes > 0 &&
            MinPlayTimeMinutes > MaxPlayTimeMinutes)
        {
            yield return new ValidationResult(
                "MinPlayTimeMinutes cannot be greater than MaxPlayTimeMinutes.",
                [nameof(MinPlayTimeMinutes), nameof(MaxPlayTimeMinutes)]);
        }

        if (PlayerRatings is null)
        {
            yield return new ValidationResult(
                "PlayerRatings cannot be null.",
                [nameof(PlayerRatings)]);
            yield break;
        }

        if (PlayerRatings.Any(rating => rating is null))
        {
            yield return new ValidationResult(
                "PlayerRatings cannot contain null items.",
                [nameof(PlayerRatings)]);
        }

        var duplicatePlayerCounts = PlayerRatings
            .Where(rating => rating is not null)
            .GroupBy(rating => rating!.PlayerCount)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        if (duplicatePlayerCounts.Length > 0)
        {
            yield return new ValidationResult(
                "Each player count may have only one rating.",
                [nameof(PlayerRatings)]);
        }

        foreach (var playerRating in PlayerRatings.Where(rating => rating is not null))
        {
            if (playerRating!.PlayerCount < MinPlayers || playerRating.PlayerCount > MaxPlayers)
            {
                yield return new ValidationResult(
                    $"Player rating count {playerRating!.PlayerCount} must be within the game's supported player range.",
                    [nameof(PlayerRatings)]);
            }
        }
    }
}
