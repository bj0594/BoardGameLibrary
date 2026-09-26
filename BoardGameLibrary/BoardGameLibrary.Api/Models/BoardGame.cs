namespace BoardGameLibrary.Api.Models;

/// <summary>
/// Represents one board game stored in the local library.
/// </summary>
public class BoardGame
{
    /// <summary>Local database identifier.</summary>
    public int Id { get; set; }

    /// <summary>Display name of the game.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Smallest supported player count.</summary>
    public int MinPlayers { get; set; }

    /// <summary>Largest supported player count.</summary>
    public int MaxPlayers { get; set; }

    /// <summary>UTC timestamp recorded when the game is added to the library.</summary>
    public DateTimeOffset CreatedAt { get; set; }
}
