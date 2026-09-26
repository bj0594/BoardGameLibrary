using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameLibrary.Api.Services;

/// <summary>
/// Contains application behaviour for reading and creating local board-game resources.
/// </summary>
public class BoardGameService(BoardGameDbContext dbContext)
{
    /// <summary>Retrieves one game by its local database identifier.</summary>
    public async Task<BoardGame?> GetAsync(
        int id,
        CancellationToken cancellationToken)
        => await dbContext.BoardGames
            .AsNoTracking()
            .SingleOrDefaultAsync(
                game => game.Id == id,
                cancellationToken);

    /// <summary>
    /// Retrieves the collection, optionally filtering to games that support the requested player count.
    /// Results are ordered consistently for predictable API responses.
    /// </summary>
    public async Task<List<BoardGame>> GetAllAsync(
        int? players,
        CancellationToken cancellationToken)
    {
        var query = dbContext.BoardGames.AsNoTracking();

        if (players.HasValue)
        {
            query = query.Where(game =>
                game.MinPlayers <= players.Value &&
                game.MaxPlayers >= players.Value);
        }

        return await query
            .OrderBy(game => game.Title)
            .ThenBy(game => game.Id)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Converts validated API input into a local entity and persists it asynchronously.
    /// </summary>
    public async Task<BoardGame> AddAsync(
        CreateBoardGameRequest request,
        CancellationToken cancellationToken)
    {
        var game = new BoardGame
        {
            // Validation guarantees Title is present; trimming keeps stored titles clean.
            Title = request.Title!.Trim(),
            MinPlayers = request.MinPlayers,
            MaxPlayers = request.MaxPlayers,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.BoardGames.Add(game);
        await dbContext.SaveChangesAsync(cancellationToken);

        return game;
    }
}
