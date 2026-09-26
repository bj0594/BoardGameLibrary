using System.ComponentModel;
using BoardGameLibrary.Api.Models;
using BoardGameLibrary.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameLibrary.Api.Controllers;

/// <summary>
/// HTTP boundary for creating, retrieving, and filtering local board games.
/// </summary>
[ApiController]
[Route("api/games")]
public class BoardGamesController(BoardGameService boardGameService) : ControllerBase
{
    /// <summary>Returns the full board-game collection, optionally filtered by player count.</summary>
    [HttpGet]
    [EndpointSummary("List board games")]
    [EndpointDescription("Returns all stored games, or only games whose supported player range includes the requested player count.")]
    [ProducesResponseType(typeof(IEnumerable<BoardGame>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BoardGame>>> GetAll(
        [FromQuery, Description("Optional positive player count. A game matches when MinPlayers <= players <= MaxPlayers.")] int? players,
        CancellationToken cancellationToken)
    {
        if (players <= 0)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid player count.",
                detail: "The players query parameter must be greater than zero.");
        }

        try
        {
            return Ok(await boardGameService.GetAllAsync(
                players,
                cancellationToken));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // Keep database details out of the HTTP response while returning a stable API error.
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Database operation failed.");
        }
    }

    /// <summary>Returns one stored board game by local identifier.</summary>
    [HttpGet("{id:int}")]
    [EndpointSummary("Get a board game")]
    [EndpointDescription("Returns one stored board game by its local database identifier.")]
    [ProducesResponseType(typeof(BoardGame), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BoardGame>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid board-game ID.",
                detail: "The board-game ID must be greater than zero.");
        }

        try
        {
            var game = await boardGameService.GetAsync(id, cancellationToken);

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

    /// <summary>Creates and persists one board game from validated JSON input.</summary>
    [HttpPost]
    [EndpointSummary("Add a board game")]
    [EndpointDescription("Creates a board game in the local library and returns the new resource with a Location header for GET /api/games/{id}.")]
    [ProducesResponseType(typeof(BoardGame), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BoardGame>> Create(
        [FromBody] CreateBoardGameRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Request body is required.",
                detail: "Provide a JSON object containing title, minPlayers, and maxPlayers.");
        }

        try
        {
            var game = await boardGameService.AddAsync(
                request,
                cancellationToken);

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
}
