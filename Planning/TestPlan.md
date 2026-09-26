# BoardGame Library API – Test Plan

## 1. Verification strategy

The automated suite is intended to prove the functional MVP. It tests the API through HTTP with an isolated SQLite test database. Internal implementation details are only inspected where behaviour alone cannot prove the requirement, especially async/non-blocking design.

The target is not maximum test count. The target is sufficient behavioural coverage that a green suite establishes the planned MVP behaviour.

## 2. Behaviour coverage

### T01 — Add game
- Valid request returns `201 Created`.
- Created resource is returned.
- `Location` points to the created resource.
- Resource is persisted with the expected fields.

### T02 — Retrieve games
- Collection returns stored games in stable order.
- Empty collection returns `200` with an empty list.
- Single resource returns `200` when found.
- Missing resource returns `404`.

### T03 — Player-count discovery
- `players=1` returns only games supporting one player.
- `players=2` returns only games supporting two players.
- Valid query with no matches returns an empty collection.
- Zero, negative, and non-numeric values return `400`.

### T04 — Validation
- Missing title returns `400` with useful validation feedback.
- Blank title returns `400`.
- Zero minimum players returns `400`.
- Zero maximum players returns `400`.
- `MinPlayers > MaxPlayers` returns `400`.
- JSON `null` request body returns `400`.
- Non-positive resource ID returns `400` and does not mutate state.

### T05 — Persistence failure
- GET database failure returns `500`.
- POST database failure returns `500`.

## 3. Test level

All listed T-cases are API/integration tests using:

- `WebApplicationFactory<Program>`.
- isolated SQLite in-memory database per test factory.
- real Controller and Service pipeline.

No live external service is required for the MVP.

## 4. Non-automated evidence

### I01 — Async I/O inspection
Verify the Controller → Service → EF Core path uses async APIs and contains no `.Result`, `.Wait()`, or equivalent blocking calls.

### I02 — Controller responsibility inspection
Verify Controllers remain focused on HTTP concerns and do not contain EF Core/database implementation.

### M01 — Manual add
Use the running API and submit a valid POST.

### M02 — Manual retrieval
Use GET collection and GET by ID.

### M03 — Manual player filtering
Use `players=1` and another positive player count.

### M04 — Manual validation
Submit an invalid request and verify the documented `400` response.

### DOC01 — README
Confirm the README accurately documents setup, migrations, running, endpoint usage, and testing.

### DOC02 — Delivery
Confirm the required repository and Canvas delivery are complete.

## 5. Completion gate

The MVP verification gate passes when:
- all automated T-cases pass;
- `dotnet build` passes;
- the current EF Core migration has been generated/applied successfully;
- M01–M04 are performed successfully;
- I01–I02 are reviewed;
- README and delivery evidence are complete.
