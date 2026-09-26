using BoardGameLibrary.Api.Models;
using BoardGameLibrary.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameLibrary.Api.Controllers;

[ApiController]
[Route("api/games")]
public class BoardGamesController(BoardGameService boardGameService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BoardGame>>> GetAll(
        [FromQuery] int? players,
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
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Database operation failed.");
        }
    }

    [HttpGet("{id:int}")]
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

    [HttpPost]
    public async Task<ActionResult<BoardGame>> Create(
        CreateBoardGameRequest? request,
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
