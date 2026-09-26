using BoardGameLibrary.Api.Models;
using BoardGameLibrary.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameLibrary.Api.Controllers;

/// <summary>
/// HTTP boundary for the board-game discovery API.
/// </summary>
[ApiController]
[Route("api/games")]
public class BoardGamesController(BoardGameService boardGameService) : ControllerBase
{
    /// <summary>Returns the library, optionally filtered and sorted for a specific use case.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BoardGame>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BoardGame>>> GetAll(
        [FromQuery] int? players,
        [FromQuery] int? maxMinutes,
        [FromQuery] string? sort,
        CancellationToken cancellationToken)
    {
        if (players <= 0)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid player count.",
                detail: "The players query parameter must be greater than zero.");
        }

        if (maxMinutes <= 0)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid maximum play time.",
                detail: "The maxMinutes query parameter must be greater than zero.");
        }

        if (string.Equals(sort, "rating", StringComparison.OrdinalIgnoreCase) && !players.HasValue)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Rating sort requires a player count.",
                detail: "Provide the players query parameter when sorting by player-count rating.");
        }

        if (!IsSupportedSort(sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid sort value.",
                detail: "The sort parameter must be title, rating, or playtime.");
        }

        try
        {
            return Ok(await boardGameService.GetAllAsync(
                players,
                maxMinutes,
                sort,
                cancellationToken));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Database operation failed.");
        }
    }

    /// <summary>Returns one game and, when requested, its rating for a selected player count.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BoardGame), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BoardGame>> GetById(
        int id,
        [FromQuery] int? players,
        CancellationToken cancellationToken)
    {
        if (id <= 0 || players <= 0)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid board-game query.",
                detail: "The board-game ID and players query parameter, when provided, must be greater than zero.");
        }

        try
        {
            var game = await boardGameService.GetAsync(id, players, cancellationToken);

            return game is null
                ? Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Board game not found.")
                : Ok(game);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Database operation failed.");
        }
    }

    /// <summary>Creates a local board game with optional player-count-specific ratings.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(BoardGame), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BoardGame>> Create(
        CreateBoardGameRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Request body is required.",
                detail: "Provide a JSON object containing the board-game fields.");
        }

        try
        {
            var game = await boardGameService.AddAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = game.Id },
                game);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Board game could not be persisted.");
        }
    }

    private static bool IsSupportedSort(string? sort) =>
        string.IsNullOrWhiteSpace(sort) ||
        sort.Trim().Equals("title", StringComparison.OrdinalIgnoreCase) ||
        sort.Trim().Equals("rating", StringComparison.OrdinalIgnoreCase) ||
        sort.Trim().Equals("playtime", StringComparison.OrdinalIgnoreCase);
}
