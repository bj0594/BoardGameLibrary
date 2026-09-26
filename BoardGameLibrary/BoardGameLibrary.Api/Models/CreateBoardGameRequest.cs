using System.ComponentModel.DataAnnotations;

namespace BoardGameLibrary.Api.Models;

/// <summary>
/// Defines and validates the client input accepted by POST /api/games.
/// </summary>
public class CreateBoardGameRequest : IValidatableObject
{
    /// <summary>Game title supplied by the client.</summary>
    [Required]
    [StringLength(200)]
    public string? Title { get; set; }

    /// <summary>Minimum number of players supported by the game.</summary>
    [Range(1, int.MaxValue)]
    public int MinPlayers { get; set; }

    /// <summary>Maximum number of players supported by the game.</summary>
    [Range(1, int.MaxValue)]
    public int MaxPlayers { get; set; }

    /// <summary>
    /// Enforces the cross-field rule that the minimum cannot exceed the maximum.
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
    }
}
