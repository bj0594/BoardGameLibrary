# BoardGame Library API

> C# / ASP.NET Core project developed from the Kodehode Project Work Template.

## 1. Project description

### What is this project?

BoardGame Library API is a C# / ASP.NET Core REST API for a personal board-game library.

The project uses BoardGameGeek as an external source of board-game information and stores a focused local representation in SQLite through Entity Framework Core.

The API is designed to make board-game discovery by player count more useful, with one-player discovery as the primary use case.

### Why does it exist?

The assignment requires a Controller-based REST API with GET and POST operations, validation, appropriate HTTP behaviour, asynchronous handling, testing, documentation, and delivery.

The selected project direction adds a concrete backend problem to those requirements. BoardGameGeek provides general board-game information and community recommendations for different player counts, but the project will provide a local API and database through which this information can be stored, queried, and used for player-count discovery.

### Main functionality

- Add a board game using its BoardGameGeek identifier.
- Retrieve board games stored in the local library.
- Retrieve a specific board game.
- Filter board games by player-count recommendation data.
- Store relevant BoardGameGeek player-count information locally.
- Support one-player discovery as the primary solo use case.

## 2. How to run

### Requirements

- .NET 10 SDK
- .NET-compatible development environment
- Access to the BoardGameGeek API for the external integration
- No separate database server is required; the project uses SQLite.

### Setup

Clone the repository and restore the .NET dependencies.

The BoardGameGeek integration will require the configuration needed for authorized API access. Do not commit API credentials or other secrets to the repository.

The SQLite database is local and is created/configured by the application as part of the database setup.

### Run

From the repository root:

    dotnet run --project BoardGameLibrary/BoardGameLibrary.Api

The exact database initialization and migration steps will be documented here once the final EF Core setup is implemented.

## 3. How to use / test

### Manual verification

The API is intended to be manually exercised through Swagger or another suitable HTTP client once the endpoints are implemented.

Manual verification will cover the required GET and POST behaviour, validation, HTTP responses, and relevant player-count discovery behaviour.

### Example

The planned core operations are:

    POST /api/games

    GET /api/games

    GET /api/games/{bggId}

    GET /api/games?players=1

The exact request bodies and finalized response formats will be documented once the API contracts are implemented and verified.

### Expected result

A successful POST should create and persist a valid local board-game resource and return the appropriate creation response.

GET requests should return the locally stored board-game data.

A `players` query should use the stored BoardGameGeek player-count recommendation data. `players=1` is the primary solo-discovery use case.

## 4. API / interface

The current planned API surface is:

### Add a board game

`POST /api/games`

Creates a local board-game resource using a BoardGameGeek identifier and the relevant data retrieved from BoardGameGeek.

### Get board games

`GET /api/games`

Returns the locally stored board-game collection.

### Get one board game

`GET /api/games/{bggId}`

Returns one locally stored board game by its BoardGameGeek identifier.

### Filter by player count

`GET /api/games?players={n}`

Returns games with BoardGameGeek community recommendation data for the requested player-count category.

`players=1` represents the project's primary solo-discovery use case.

### Request examples

Final request examples will be added after the endpoint contracts are implemented and verified.

### Response examples

Final response examples will be added after the endpoint contracts are implemented and verified.

## 5. Configuration / external dependencies

### BoardGameGeek

BoardGameGeek is used as an external source for relevant board-game information.

The application imports only the information required by the project instead of mirroring the complete BoardGameGeek data model.

BoardGameGeek API access and any required credentials must be configured locally.

Do not commit credentials, API tokens, or other secrets to the repository.

### SQLite

The project uses a local SQLite database through Entity Framework Core.

No separate database server is required.

The final connection/database configuration will be documented here after the EF Core setup is implemented.

## 6. Project structure

    BoardGame Library API/
    ├── BoardGameLibrary/
    │   ├── BoardGameLibrary.Api/
    │   ├── BoardGameLibrary.Tests/
    │   └── BoardGameLibrary.slnx
    ├── Planning/
    │   ├── Planning.md
    │   └── TestPlan.md
    ├── TestTemplates/
    ├── README.md
    └── .gitignore

### Main project folders

- `BoardGameLibrary.Api/` — production API.
- `BoardGameLibrary.Tests/` — automated xUnit tests.
- `Planning/` — project planning and verification design.
- `TestTemplates/` — reusable test starting templates.

## 7. Verification summary

The project uses multiple forms of verification according to the requirements and project direction.

Automated verification will use xUnit.

Manual API verification will be performed through Swagger or another suitable HTTP client.

Integration verification will cover relevant boundaries such as the local SQLite database and BoardGameGeek integration.

Implementation inspection will be used where behaviour cannot be meaningfully established from endpoint output alone, particularly asynchronous and non-blocking I/O requirements.

The final verification summary will be updated after implementation and testing are complete.

## 8. Assignment / delivery notes

- Repository: `https://github.com/bj0594/BoardGameLibrary`
- Submission method: GitHub repository link submitted through Canvas.
- Deadline: 27 September 2026.
- Required assignment areas include the Controller-based REST API, GET and POST behaviour, validation, HTTP behaviour, asynchronous/non-blocking handling, documentation, verification, and delivery.

## 9. Development workspace

This project was created from the Kodehode Project Work Template.

Use the workspace files as follows:

- `Planning/Planning.md` — project understanding, requirements, scope, project direction, decisions, behaviour contracts, domain/rules, and current design.
- `Planning/TestPlan.md` — verification strategy, coverage, test design, evidence, and implementation-readiness.
- `TestTemplates/` — reusable xUnit starting skeletons. Use only when a suitable template is actually helpful.

`Planning.md` is the project planning truth.

`TestPlan.md` is the verification and test-design truth.

A concrete test file is the executable source of truth for that test's implemented behaviour.

If Todoist is used, Todoist tracks work and progress. It does not replace these project artefacts or become a second source of truth.

### Final-project cleanup

Before delivery:

- Replace this development workspace section with only the information that remains useful to a reader.
- Remove unused test templates.
- Remove template-specific instructions that no longer belong in the final README.
- Complete final run, database, API, request, response, and verification documentation.
- Make sure the README describes the implemented project rather than its original scaffold.

## 10. Not relevant / intentionally omitted

The following areas are intentionally outside the current project scope:

- Full BoardGameGeek data mirroring.
- Authentication and authorization for the BoardGame Library API.
- Background synchronization jobs.
- Caching infrastructure.
- Redis or other additional infrastructure.
- Update and delete operations unless a concrete project requirement later justifies them.
- Advanced pagination or filtering beyond the selected player-count functionality unless it provides meaningful value.

---

**Main rule:** The final README should contain the information another developer or evaluator needs to understand, run, use, and verify the actual project. It should not become a second `Planning.md` or `TestPlan.md`.