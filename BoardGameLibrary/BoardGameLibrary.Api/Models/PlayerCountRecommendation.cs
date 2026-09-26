using System.Text.Json.Serialization;

namespace BoardGameLibrary.Api.Models;

public class PlayerCountRecommendation
{
    public string PlayerCount { get; set; } = string.Empty;
    public int BestVotes { get; set; }
    public int RecommendedVotes { get; set; }
    public int NotRecommendedVotes { get; set; }

    public int BoardGameId { get; set; }
    [JsonIgnore]
    public BoardGame BoardGame { get; set; } = null!;
}
