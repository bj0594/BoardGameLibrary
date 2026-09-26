# BoardGame Library API

A C# / ASP.NET Core REST API for a personal board-game library.

The API uses BoardGameGeek as an external source of board-game information and stores a focused local representation in SQLite through Entity Framework Core. The primary discovery use case is finding stored games by BoardGameGeek player-count recommendation data, with `players=1` serving as the main solo-discovery query.

## 1. Project description

### What is this project?

The API lets a local board-game library add games by BoardGameGeek ID, persist the relevant BGG data locally, retrieve stored games, and query them by player-count recommendation category.

The project demonstrates Controller-based REST API design, asynchronous external and database I/O, Entity Framework Core, validation, error handling, external-data mapping, and automated API/integration testing with xUnit.

### Why does it exist?

The project fulfils the Kodehode REST API assignment while adding a concrete backend use case. BoardGameGeek exposes useful player-count recommendation data, but the project keeps a focused local representation that can be queried through its own API.

### Main functionality

- Add a board game from its BoardGameGeek ID.
- Persist relevant BGG data in SQLite.
- Retrieve the stored board-game collection or a specific game.
- Filter the collection by BGG player-count recommendation bucket.
- Support `players=1` as the primary solo-discovery use case.

## 2. How to run

### Requirements

- .NET 10 SDK.
- Internet access for BoardGameGeek requests when adding games.
- Valid BoardGameGeek API authorization configured locally if required by the current BGG API access.

No separate database server is required. SQLite is stored locally in `boardgamelibrary.db`.

### Setup

Clone the repository and restore dependencies:

    dotnet restore

BoardGameGeek authorization should be supplied through local configuration and must not be committed to Git.

The SQLite schema is managed through Entity Framework Core migrations. Apply the current migrations before starting the application.

### Run

From the repository root:

    dotnet run --project BoardGameLibrary/BoardGameLibrary.Api

The OpenAPI document is exposed in the Development environment.

### EF Core migrations

From the repository's `BoardGameLibrary` directory, after confirming the model is final:

    dotnet ef migrations add InitialCreate --project BoardGameLibrary.Api --startup-project BoardGameLibrary.Api
    dotnet ef database update --project BoardGameLibrary.Api --startup-project BoardGameLibrary.Api

Review the generated migration before applying it.

## 3. How to use / test

### Manual verification

Start the API and use the generated OpenAPI endpoint or another HTTP client such as the `.http` file, cURL, or Postman.

The repository's `BoardGameLibrary.Api.http` file contains representative requests for the collection GET, single-resource GET, player-count filtering, and POST operations.

### Example

Add a board game:

    POST /api/games

    {
      "bggId": 12345
    }

Filter for the one-player recommendation bucket:

    GET /api/games?players=1

Filter for an open-ended BGG bucket:

    GET /api/games?players=4%2B

### Expected result

A successful POST returns `201 Created`, includes the created resource, and points to the created resource with the response `Location`.

GET requests return the local library data.

A `players` query filters against the stored BGG player-count recommendation category. The API does not convert the BGG community signal into an objective solo-quality rating.

### Automated tests

Run the test suite from the `BoardGameLibrary` solution directory:

    dotnet test

The automated suite uses a controlled BoardGameGeek boundary and an in-memory SQLite database so the tests do not depend on the live BGG service.

## 4. API / interface

### Add a board game

`POST /api/games`

Creates a local board-game resource using a BoardGameGeek ID and the relevant external data.

### Get all board games

`GET /api/games`

Returns the local board-game collection.

### Get one board game

`GET /api/games/{bggId}`

Returns one local board-game resource.

### Filter by player count

`GET /api/games?players={n}`

Filters the local collection by a BGG player-count recommendation bucket.

Values may be numeric categories such as `1` or `2`, or open-ended categories such as `4+`.

### Request examples

See `BoardGameLibrary/BoardGameLibrary.Api/BoardGameLibrary.Api.http`.

### Response examples

The API returns JSON representations of the local board-game model.

## 5. Configuration / external dependencies

### BoardGameGeek

BoardGameGeek is the external source for the imported board-game data.

The application requests the selected game information and the `suggested_numplayers` recommendation data, then maps only the required fields into the local model.

Any required BoardGameGeek authorization token must be supplied locally and never committed to the repository.

### SQLite

The API uses SQLite through Entity Framework Core.

The default connection string is:

    Data Source=boardgamelibrary.db

The production/local database schema is managed through EF Core migrations. Tests use an isolated in-memory SQLite database and create their test schema independently.

## 6. Project structure

    BoardGame Library API/
    ├── BoardGameLibrary/
    │   ├── BoardGameLibrary.Api/
    │   │   ├── Controllers/
    │   │   ├── Data/
    │   │   ├── Model/
    │   │   └── Services/
    │   ├── BoardGameLibrary.Tests/
    │   └── BoardGameLibrary.slnx
    ├── Planning/
    │   ├── Planning.md
    │   └── TestPlan.md
    └── README.md

## 7. Verification summary

The important behaviours are verified through:

- xUnit API/integration tests.
- Controlled SQLite persistence tests.
- Controlled BoardGameGeek client tests.
- Manual HTTP/API verification.
- Inspection of asynchronous I/O paths and Controller responsibilities.

The test suite covers creation, retrieval, player-count discovery, validation, external failures, persistence failures, duplicate handling, and BGG XML mapping.

## 8. Assignment / delivery notes

- Repository: `https://github.com/bj0594/BoardGameLibrary`
- Submission: GitHub repository link in Canvas.
- Deadline: 27 September 2026.
- Required assignment areas include a meaningful model, Controller-based GET and POST endpoints, validation, appropriate HTTP responses, asynchronous/non-blocking I/O, README documentation, and manual endpoint verification.

## 9. Development notes

`Planning/Planning.md` is the current project planning truth.

`Planning/TestPlan.md` is the current verification and test-design truth.

Concrete test files are the executable source of truth for their implemented test behaviours.

## 10. Intentionally omitted

The current MVP does not include:

- full BoardGameGeek data mirroring;
- authentication or authorization for the BoardGame Library API;
- background synchronization;
- caching infrastructure;
- Redis or other additional infrastructure;
- update/delete endpoints;
- advanced pagination or sorting without a concrete requirement.
