# BoardGame Library API

A small C# / ASP.NET Core REST API for managing and discovering a personal board-game library.

The project is intentionally narrow: it demonstrates Controllers, validation, async database I/O, EF Core, SQLite, API integration tests, and a useful GET-based discovery feature rather than a large CRUD surface.

## What the API is for

The core question is:

> Which games in my library are the best fit for the number of people I have and the time available?

The API therefore supports player-count filtering, maximum play-time filtering, and sorting by the local rating for the selected player count.

A game can be rated differently for different player counts. For example, the same game can be rated 8.1 for two players and 9.2 for four players. When `players=4` is selected, the response exposes the four-player rating.

## How to run

Requirements:

- .NET 10 SDK
- No external service, API account, or API key at runtime
- SQLite database managed through EF Core migrations

From the `BoardGameLibrary` solution directory:

    dotnet restore
    dotnet ef database update --project BoardGameLibrary.Api --startup-project BoardGameLibrary.Api
    dotnet run --project BoardGameLibrary.Api

The repository contains the current baseline migration. Do not create a new migration just to run the project. If you have a local `boardgamelibrary.db` created by an older version of the project, delete that file once before running `dotnet ef database update`.

In Development, the application seeds a demonstration library of ten games when the database is empty. Existing data is never replaced by the seeder.

The Development root opens Swagger UI at:

    http://localhost:5006/

The launch settings also support HTTPS on the configured HTTPS port.

## API surface

The project intentionally keeps the HTTP surface to GET and POST, as required by the assignment.

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/games` | Browse the library. |
| GET | `/api/games/{id}` | Retrieve one game. |
| POST | `/api/games` | Add a game with optional player-count ratings. |

### GET discovery parameters

`players` filters to games whose supported range contains the requested player count.

`maxMinutes` filters to games whose maximum stored play time is within the requested limit.

`sort` accepts:

- `title` — alphabetical order; this is the default.
- `rating` — highest rating for the selected `players` value first.
- `playtime` — shortest maximum play time first.

`sort=rating` requires `players`, because a rating is specific to a player count.

A representative request is:

    GET /api/games?players=4&maxMinutes=90&sort=rating

The response includes `selectedPlayerCount` and `selectedPlayerRating` so the reason a game ranked where it did is visible directly in the API response.

### GET one game with a selected player count

    GET /api/games/1?players=3

The response includes the game's stored player-count ratings and identifies the selected three-player rating.

### POST example

    POST /api/games
    Content-Type: application/json

    {
      "title": "Example Game",
      "minPlayers": 2,
      "maxPlayers": 4,
      "minPlayTimeMinutes": 45,
      "maxPlayTimeMinutes": 90,
      "playerRatings": [
        { "playerCount": 2, "rating": 7.5 },
        { "playerCount": 3, "rating": 8.5 },
        { "playerCount": 4, "rating": 8.0 }
      ]
    }

Successful creation returns `201 Created`, the created resource, and a `Location` header pointing to the new resource.

## Seed data and BGG provenance

The Development seed contains ten real board games and a snapshot of BGG metadata: BGG ID, title, player range, play-time range, average rating, and community best-with information.

The stored BGG fields are static snapshot data; the running application does not call BGG and therefore does not require BGG credentials.

The per-player-count `PlayerCountRating` values are deliberately local demo/library ratings. They are not presented as official BGG ratings. This distinction keeps source data and local user judgement separate.

The seed source URLs are stored alongside the BGG metadata so the snapshot can be reviewed or refreshed deliberately rather than silently changing at runtime. BoardGameGeek is credited as the source of the imported snapshot data; see the current [BGG XML API Terms of Use](https://boardgamegeek.com/wiki/page/XML%20API%20Terms%20of%20Use) before reusing or redistributing the data in another context.

## Database

Production/local development uses SQLite with EF Core migrations.

The repository contains the initial migration. Schema changes should be made by generating a new migration and applying it with `dotnet ef database update`.

Tests use a separate SQLite in-memory connection, so tests never depend on the developer's local database.

## Testing

Run the complete automated API suite:

    dotnet test

Build the solution:

    dotnet build

The tests exercise the real Controller → Service → EF Core pipeline and cover creation, retrieval, player-count discovery, player-count-specific ratings, play-time filtering, sorting, validation, and database failure handling.

Manual verification can be performed through Swagger UI, the `.http` file, cURL, Postman, or another HTTP client.

## Project structure

    BoardGameLibrary/
    ├── BoardGameLibrary.Api/
    │   ├── Controllers/
    │   ├── Data/
    │   ├── Model/
    │   ├── SeedData/
    │   ├── Services/
    │   └── Migrations/           generated by EF Core
    ├── BoardGameLibrary.Tests/
    ├── Planning/
    └── README.md

## Verification checklist

Before delivery:

- `dotnet test` passes.
- `dotnet build` passes.
- The current EF Core migrations are applied successfully.
- Swagger can be opened from the Development root.
- GET collection, GET by ID, POST, filtering, sorting, and validation have been manually exercised.
- The README matches the actual project.

## Assignment delivery

- Repository: `https://github.com/bj0594/BoardGameLibrary`
- Submission: GitHub repository link in Canvas.
- Deadline: 27 September 2026.

## Development workspace

- `Planning/Planning.md` — project direction, requirements, scope, behaviour contracts, and decisions.
- `Planning/TestPlan.md` — verification strategy and automated/manual test design.
- Concrete test files — executable API behaviour.