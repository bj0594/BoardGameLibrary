# BoardGame Library API – Test Plan

## 1. Verification strategy

The automated suite is the executable MVP contract. Tests run the real Controller → Service → EF Core pipeline against isolated SQLite in-memory databases.

The goal is sufficient behavioural coverage, not maximum test count.

## 2. Automated coverage

### Create and persistence
- Valid POST returns `201 Created`.
- Created resource contains the requested data.
- Player-count ratings are persisted.
- `Location` points to a retrievable resource.

### Retrieval
- Collection returns stored resources in stable default order.
- Empty collection returns `200` with an empty array.
- GET by ID returns a stored game.
- GET by ID can expose the selected player-count rating.
- Missing resources return `404`.

### Player-count discovery
- Compatible games are returned.
- Incompatible games are excluded.
- Minimum/maximum player boundaries work.
- The selected player-count rating changes with `players`.
- No matches returns an empty collection.

### Filtering and sorting
- `maxMinutes` excludes games whose maximum play time is too long.
- Combined player/time filtering works.
- `sort=rating` orders matching games by the selected player-count rating.
- `sort=playtime` orders by maximum play time.
- `sort=rating` without `players` returns `400`.

### Validation
- Missing or blank title.
- Invalid player range.
- Invalid play-time range.
- Title over 200 characters.
- Invalid player-count rating.
- Player-count rating outside the game's player range.
- Duplicate player-count ratings.
- Null JSON body.
- Invalid resource ID.
- Invalid `players` query.
- Invalid `maxMinutes` query.
- Invalid `sort` query.
- Invalid POST requests do not persist state.

### Failure handling
- GET database failure returns `500`.
- POST database failure returns `500`.

## 3. Integration level

All functional tests are API/integration tests using:

- `WebApplicationFactory<Program>`;
- isolated SQLite in-memory database;
- the real Controller and Service pipeline;
- no live external dependency.

## 4. Non-automated evidence

### I01 — Async I/O inspection
Verify EF Core access uses async APIs and there are no blocking `.Result`, `.Wait()`, or equivalent calls.

### I02 — Controller responsibility inspection
Verify Controllers remain HTTP-focused and do not contain EF Core implementation.

### M01 — Manual library browse
Use Swagger to view the seeded development library.

### M02 — Manual player-count discovery
Try a few `players` values and confirm the selected rating changes accordingly.

### M03 — Manual constrained discovery
Use a combined query such as `players=4&maxMinutes=90&sort=rating`.

### M04 — Manual POST and validation
Create a valid game and submit an invalid request.

### DOC01 — README
Confirm setup, migrations, Swagger, endpoint examples, seed behaviour, and testing instructions match the project.

### DOC02 — Delivery
Confirm repository and Canvas delivery are complete.

## 5. Completion gate

The gate passes when the automated suite and build are green, the latest migration is applied, Swagger and the main discovery flows have been manually verified, async/controller inspections are complete, and README/delivery evidence is current.
