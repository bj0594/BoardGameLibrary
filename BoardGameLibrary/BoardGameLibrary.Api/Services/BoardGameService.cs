using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameLibrary.Api.Services;

public class BoardGameService(BoardGameDbContext dbContext)
{
    public async Task<BoardGame?> GetAsync(
        int id,
        CancellationToken cancellationToken)
        => await dbContext.BoardGames
            .AsNoTracking()
            .SingleOrDefaultAsync(
                game => game.Id == id,
                cancellationToken);

    public async Task<List<BoardGame>> GetAllAsync(
        int? players,
        CancellationToken cancellationToken)
    {
        var query = dbContext.BoardGames
            .AsNoTracking()
            .AsQueryable();

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

    public async Task<BoardGame> AddAsync(
        CreateBoardGameRequest request,
        CancellationToken cancellationToken)
    {
        var game = new BoardGame
        {
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
