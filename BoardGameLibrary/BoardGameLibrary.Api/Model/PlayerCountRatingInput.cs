using System.ComponentModel.DataAnnotations;

namespace BoardGameLibrary.Api.Models;

/// <summary>
/// Request-side representation of one player-count-specific library rating.
/// </summary>
public class PlayerCountRatingInput : IValidatableObject
{
    /// <summary>The player count the rating applies to.</summary>
    [Range(1, int.MaxValue)]
    public int PlayerCount { get; set; }

    /// <summary>Local rating from 0 to 10.</summary>
    [Range(typeof(decimal), "0", "10")]
    public decimal Rating { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (decimal.Round(Rating, 1) != Rating)
        {
            yield return new ValidationResult(
                "Rating may contain at most one decimal place.",
                [nameof(Rating)]);
        }
    }
}
