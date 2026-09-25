namespace BoardGameLibrary.Api.Models;

public class BoardGame
{
    public int BggId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public double? BggAverageRating { get; set; }
    public int? BggRatingCount { get; set; }
    public List<PlayerCountRecommendation> PlayerCountRecommendations { get; set; } = [];
}
