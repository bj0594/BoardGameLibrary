using BoardGameLibrary.Api.Models;

namespace BoardGameLibrary.Api.Services;

public interface IBoardGameGeekClient
{
    Task<BoardGame?> GetBoardGameAsync(int bggId, CancellationToken cancellationToken = default);
}
