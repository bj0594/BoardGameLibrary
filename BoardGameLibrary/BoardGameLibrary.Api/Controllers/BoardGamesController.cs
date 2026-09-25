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
        if (players is <= 0)
        {
            return BadRequest(new { error = "The players query parameter must be greater than zero." });
        }

        try
        {
            return Ok(await boardGameService.GetAllAsync(players, cancellationToken));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Database operation failed.");
        }
    }

    [HttpGet("{bggId:int}")]
    public async Task<ActionResult<BoardGame>> GetById(
        int bggId,
        CancellationToken cancellationToken)
    {
        if (bggId <= 0)
        {
            return BadRequest(new { error = "The BGG ID must be greater than zero." });
        }

        try
        {
            var game = await boardGameService.GetAsync(bggId, cancellationToken);
            return game is null ? NotFound() : Ok(game);
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
        CreateBoardGameRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var game = await boardGameService.AddAsync(request.BggId, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { bggId = game.BggId },
                game);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { error = exception.Message });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { error = exception.Message });
        }
        catch (HttpRequestException)
        {
            return Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "BoardGameGeek could not be reached or returned an unsuccessful response.");
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Board game could not be persisted.");
        }
    }
}
