using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameLibrary.Api.Services;

/// <summary>
/// Application-level operations for creating and discovering board games.
/// Database access stays here so Controllers remain focused on HTTP concerns.
/// </summary>
public class BoardGameService(BoardGameDbContext dbContext)
{
    private static readonly string[] SupportedSorts = ["title", "rating", "playtime"];

    public async Task<BoardGame?> GetAsync(
        int id,
        int? players,
        CancellationToken cancellationToken)
    {
        var game = await dbContext.BoardGames
            .Include(boardGame => boardGame.PlayerCountRatings)
            .AsNoTracking()
            .SingleOrDefaultAsync(
                boardGame => boardGame.Id == id,
                cancellationToken);

        SetSelectedRating(game, players);
        return game;
    }

    public async Task<List<BoardGame>> GetAllAsync(
        int? players,
        int? maxMinutes,
        string? sort,
        CancellationToken cancellationToken)
    {
        IQueryable<BoardGame> query = dbContext.BoardGames
            .Include(game => game.PlayerCountRatings)
            .AsNoTracking();

        if (players.HasValue)
        {
            query = query.Where(game =>
                game.MinPlayers <= players.Value &&
                game.MaxPlayers >= players.Value);
        }

        if (maxMinutes.HasValue)
        {
            query = query.Where(game =>
                game.MaxPlayTimeMinutes <= maxMinutes.Value);
        }

        var normalizedSort = string.IsNullOrWhiteSpace(sort)
            ? null
            : sort.Trim().ToLowerInvariant();

        query = normalizedSort switch
        {
            null or "" or "title" => query
                .OrderBy(game => game.Title)
                .ThenBy(game => game.Id),

            "playtime" => query
                .OrderBy(game => game.MaxPlayTimeMinutes)
                .ThenBy(game => game.Title)
                .ThenBy(game => game.Id),

            "rating" when players.HasValue => query
                .OrderBy(game => game.Title)
                .ThenBy(game => game.Id),

            _ => throw new ArgumentException(
                $"Unsupported sort value. Use one of: {string.Join(", ", SupportedSorts)}.",
                nameof(sort))
        };

        var games = await query.ToListAsync(cancellationToken);

        foreach (var game in games)
        {
            SetSelectedRating(game, players);
        }

        if (normalizedSort == "rating" && players.HasValue)
        {
            games = games
                .OrderByDescending(game => game.SelectedPlayerRating.HasValue)
                .ThenByDescending(game => game.SelectedPlayerRating)
                .ThenBy(game => game.Title)
                .ThenBy(game => game.Id)
                .ToList();
        }

        return games;
    }

    public async Task<BoardGame> AddAsync(
        CreateBoardGameRequest request,
        CancellationToken cancellationToken)
    {
        var game = new BoardGame
        {
            Title = request.Title!.Trim(),
            MinPlayers = request.MinPlayers,
            MaxPlayers = request.MaxPlayers,
            MinPlayTimeMinutes = request.MinPlayTimeMinutes,
            MaxPlayTimeMinutes = request.MaxPlayTimeMinutes,
            CreatedAt = DateTimeOffset.UtcNow,
            PlayerCountRatings = request.PlayerRatings
                .Select(rating => new PlayerCountRating
                {
                    PlayerCount = rating.PlayerCount,
                    Rating = rating.Rating
                })
                .ToList()
        };

        dbContext.BoardGames.Add(game);
        await dbContext.SaveChangesAsync(cancellationToken);

        return game;
    }

    private static void SetSelectedRating(BoardGame? game, int? players)
    {
        if (game is null || !players.HasValue)
        {
            return;
        }

        game.SelectedPlayerCount = players.Value;
        game.SelectedPlayerRating = game.PlayerCountRatings
            .Where(rating => rating.PlayerCount == players.Value)
            .Select(rating => (decimal?)rating.Rating)
            .SingleOrDefault();
    }
}