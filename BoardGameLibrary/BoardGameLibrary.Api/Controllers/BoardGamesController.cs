using System.Globalization;
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
        [FromQuery] string? players,
        CancellationToken cancellationToken)
    {
        var normalizedPlayers = players?.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedPlayers) &&
            !IsValidPlayerCountBucket(normalizedPlayers))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid player-count query.",
                detail: "The players query parameter must be a positive player count or a BGG bucket such as '4+'.");
        }

        try
        {
            return Ok(await boardGameService.GetAllAsync(
                normalizedPlayers,
                cancellationToken));
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
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid BGG ID.",
                detail: "The BGG ID must be greater than zero.");
        }

        try
        {
            var game = await boardGameService.GetAsync(
                bggId,
                cancellationToken);

            return game is null
                ? NotFound()
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
        CreateBoardGameRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var game = await boardGameService.AddAsync(
                request.BggId,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { bggId = game.BggId },
                game);
        }
        catch (InvalidOperationException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Board game already exists.",
                detail: exception.Message);
        }
        catch (KeyNotFoundException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "BoardGameGeek game not found.",
                detail: exception.Message);
        }
        catch (HttpRequestException)
        {
            return Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "BoardGameGeek request failed.",
                detail: "The external BoardGameGeek service could not be reached or returned an unsuccessful response.");
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Board game could not be persisted.");
        }
    }

    private static bool IsValidPlayerCountBucket(string value)
    {
        var numericPart = value.EndsWith('+')
            ? value[..^1]
            : value;

        return int.TryParse(
            numericPart,
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out var playerCount)
            && playerCount > 0;
    }
}
