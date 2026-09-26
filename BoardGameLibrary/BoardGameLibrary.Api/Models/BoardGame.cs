namespace BoardGameLibrary.Api.Models;

public class BoardGame
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
