# BoardGame Library API – Planning

## 0. Project context

### Goal
Build a small Controller-based REST API for a personal board-game library that is useful for discovering the best-fit game for a specific group size and time limit.

### Problem / use case
A library is more useful when it can answer a concrete planning question: **what should we play with N people when we have at most M minutes?** A game's suitability can also vary by player count, so the library stores a separate local rating for each supported player count.

### Stakeholder
Course evaluator and portfolio visitors.

### Mandatory assignment constraints
- C# / ASP.NET Core.
- Controller-based REST API.
- Meaningful data model.
- GET endpoint(s).
- POST endpoint.
- POST input validation and clear errors.
- Appropriate HTTP status codes.
- Asynchronous, non-blocking I/O.
- README with run/test instructions.
- GitHub repository submitted through Canvas.

### Selected extensions
- SQLite persistence.
- Entity Framework Core.
- Async database access.
- xUnit API/integration tests.
- Player-count filtering.
- Maximum play-time filtering.
- Sorting by title, play time, or player-count rating.
- Development seed data for immediate demonstration.
- OpenAPI + Swagger UI for interactive verification.

## 1. Direction

### Chosen direction
**Personal Board Game Library & Discovery API.**

### Why
The domain remains small enough for the assignment but now demonstrates a real discovery use case rather than storage alone. GET is the main product surface: the selected player count changes which games are returned and which player-count-specific rating is surfaced.

### Explicitly excluded
- Live BoardGameGeek integration at runtime.
- Authentication/authorization.
- Update/delete endpoints.
- Repository pattern without a concrete responsibility.
- Background jobs and caching.
- Pagination until dataset size creates a real need.
- Additional domain fields that do not support the discovery use case.

## 2. Domain model

### BoardGame
- `Id` — local primary key.
- `BggId` — optional source identifier for imported snapshot data.
- `Title` — required, maximum 200 characters.
- `MinPlayers` / `MaxPlayers` — supported player range.
- `MinPlayTimeMinutes` / `MaxPlayTimeMinutes` — stored play-time range.
- `BggAverageRating` — source snapshot metadata only.
- `BggBestWith` — source snapshot summary only.
- `BggUrl` — source reference for snapshot provenance.
- `CreatedAt` — UTC application timestamp.
- `PlayerCountRatings` — local rating per supported player count.

### PlayerCountRating
- `BoardGameId` + `PlayerCount` — composite key.
- `Rating` — local 0–10 rating for that exact player count.

The local player-count rating is deliberately distinct from BGG's overall rating. It is a library/user judgement, not a claim about an official BGG player-count rating.

## 3. API contract

### POST /api/games
Creates a local board game and any supplied player-count ratings.

Validation includes:
- required/non-blank title;
- supported player range;
- play-time range;
- rating range 0–10;
- unique player-count ratings;
- player-count ratings inside the supported player range.

Success: `201 Created` with a `Location` header.

### GET /api/games
Returns the library.

Optional query parameters:
- `players` — only games supporting the requested count.
- `maxMinutes` — only games whose maximum stored play time fits within the limit.
- `sort=title|rating|playtime` — title is default; rating requires `players`.

When `players` is supplied, each returned game exposes `selectedPlayerCount` and `selectedPlayerRating`.

### GET /api/games/{id}
Returns one game. The optional `players` parameter selects the corresponding player-count rating in the response.

## 4. Behaviours

### B01 — Add a board game
A valid request persists a valid local resource and returns `201 Created` with a usable location.

### B02 — Retrieve board games
GET returns current local state without mutating it.

### B03 — Discover by player count
A requested player count filters out incompatible games and selects the matching local rating.

### B04 — Discover by constraints
`maxMinutes` and `sort` allow a useful shortlist, including combined queries such as `players=4&maxMinutes=90&sort=rating`.

### B05 — Reject invalid input
Invalid POST data and invalid query values return clear `400` responses without creating invalid state.

### B06 — Handle persistence failures
Relevant database failures return safe `500` responses without exposing internal details.

### B07 — Provide immediate demo data
Development startup seeds ten real game records when the local database is empty. Existing data is never overwritten.

## 5. Design responsibilities

### Controller
Owns routing, API-boundary validation, status codes, and HTTP responses.

### Service
Owns query/filter/sort behaviour, application rules, mapping of selected player-count ratings, and async EF Core access.

### Persistence
EF Core + SQLite. Production/local schema is managed through EF Core migrations. Tests use isolated in-memory SQLite.

### Seed data
A Development-only seeder provides an immediate portfolio/demo dataset. BGG snapshot metadata is static; the application has no live BGG dependency.

## 6. Data provenance

The seed includes ten real board games with BGG identifier, player range, play-time range, average rating, best-with summary, and source URL. The snapshot is intended to make the demo reproducible rather than requiring a live external service.

The local player-count ratings are demonstration/library data. They are not presented as BGG-generated ratings.

## 7. Source-of-truth rule

- `Planning/Planning.md` — project facts, requirements, scope, decisions, behaviour contracts, and current design.
- `Planning/TestPlan.md` — verification strategy and concrete test design.
- Concrete test files — executable API behaviour.
- Todoist, if used — work status only.

## 8. Readiness gate

Before delivery:
- [ ] Automated test suite is green.
- [ ] `dotnet build` is green.
- [ ] Latest EF Core migration has been generated and applied.
- [ ] Swagger opens from the Development root.
- [ ] GET discovery scenarios have been manually verified.
- [ ] POST and validation have been manually verified.
- [ ] Async/non-blocking path has been inspected.
- [ ] README matches the actual project.
- [ ] GitHub/Canvas delivery is complete.
