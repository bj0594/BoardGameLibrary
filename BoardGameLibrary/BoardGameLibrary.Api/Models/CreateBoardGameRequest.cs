using System.ComponentModel.DataAnnotations;

namespace BoardGameLibrary.Api.Models;

public class CreateBoardGameRequest : IValidatableObject
{
    [Required]
    [StringLength(200)]
    public string? Title { get; set; }

    [Range(1, int.MaxValue)]
    public int MinPlayers { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxPlayers { get; set; }

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
