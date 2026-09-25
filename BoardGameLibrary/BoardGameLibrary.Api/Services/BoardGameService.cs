using BoardGameLibrary.Api.Data;
using BoardGameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardGameLibrary.Api.Services;

public class BoardGameService(BoardGameDbContext dbContext, IBoardGameGeekClient bggClient)
{
    public async Task<BoardGame?> GetAsync(int bggId, CancellationToken cancellationToken)
        => await dbContext.BoardGames
            .Include(game => game.PlayerCountRecommendations)
            .SingleOrDefaultAsync(game => game.BggId == bggId, cancellationToken);

    public async Task<List<BoardGame>> GetAllAsync(int? players, CancellationToken cancellationToken)
    {
        var query = dbContext.BoardGames
            .Include(game => game.PlayerCountRecommendations)
            .AsQueryable();

        if (players.HasValue)
        {
            query = query.Where(game => game.PlayerCountRecommendations
                .Any(recommendation => recommendation.PlayerCount == players.Value.ToString()));
        }

        return await query
            .OrderBy(game => game.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<BoardGame> AddAsync(int bggId, CancellationToken cancellationToken)
    {
        var existing = await dbContext.BoardGames
            .AnyAsync(game => game.BggId == bggId, cancellationToken);

        if (existing)
        {
            throw new InvalidOperationException("A board game with this BGG ID already exists.");
        }

        var game = await bggClient.GetBoardGameAsync(bggId, cancellationToken);

        if (game is null)
        {
            throw new KeyNotFoundException("The requested BoardGameGeek game was not found.");
        }

        dbContext.BoardGames.Add(game);
        await dbContext.SaveChangesAsync(cancellationToken);

        return game;
    }
}
