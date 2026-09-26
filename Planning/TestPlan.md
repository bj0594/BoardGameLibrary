# BoardGame Library API – TEST PLAN

## How to use this file

- Define how important requirements and behaviours will be proven before implementation begins.
- Keep verification design separate from production implementation.
- Reference behaviour contracts from `Planning.md`; do not rewrite them here.
- Design verification far enough that implementation can begin without inventing missing expectations.
- Test files may be created in a batch or incrementally; verification design is independent of that choice.
- Update this file when verification strategy, coverage, test design, or implementation readiness changes.
- `Not relevant` is valid when a verification method genuinely does not apply.

---

# 1. VERIFICATION STRATEGY

## Verification goal

The project must provide evidence that:

- valid board games can be added through the API and persisted locally;
- required BoardGameGeek data is retrieved and mapped into the local model;
- stored games can be retrieved through GET;
- player-count discovery follows the finalized query and data rules, with `players=1` serving as the primary solo-discovery use case;
- invalid requests are rejected without invalid persisted state;
- relevant external and database failures are handled according to their contracts;
- required HTTP and asynchronous/non-blocking behaviour is satisfied;
- required documentation, manual verification, and delivery are complete.

`Planning.md` owns what must be true. This file owns how it will be proven.

## Evidence types

- **T – Automated test:** Automated verification when it provides useful evidence.
- **I – Implementation inspection:** Technical properties not fully observable from API output, especially async/non-blocking implementation.
- **M – Manual observation:** Swagger, cURL, Postman, or another suitable API client.
- **INT – Integration evidence:** A real or controlled interaction with BGG or the SQL database.
- **DOC – Documentation / delivery evidence:** README, repository, configuration, and GitHub/Canvas delivery.

Use only the evidence needed to establish the relevant contract.

## Automated testing decision

- **Selected:** Yes.
- **Why:** xUnit is a fixed project-workflow decision even though the assignment makes automated testing optional.
- **Test project:** `BoardGameLibrary.Tests`

---

# 2. COVERAGE MAP

`Requirement → behaviour → verification / evidence`

- **R1 →** B01 / B02 / B03 / B04 → INT / M / T
- **R2 →** B02 / B03 → M / T
- **R3 →** B01 → M / INT / T
- **R4 →** B04 → M / T
- **R5 →** B04 / B05 → M / INT / T
- **R6 →** B01 → M / T
- **R7 →** B01 / B02 / B03 / B05 → I / INT / T
- **R8 →** B01 / B02 / B05 → I / INT / T
- **R9 →** B01 / B02 / B05 → I / T
- **R10 →** B01 / B02 / B03 / B04 / B05 → M / INT / T
- **R11 →** B04 / B05 → M / INT / T
- **R12 →** Delivery → DOC
- **R13 →** B01 / B02 / B03 / B04 → I / M / INT / T
- **R14 →** Delivery → DOC
- **R15 →** Manual endpoint verification → M
- **R16 →** Delivery → DOC
- **R17 →** B01 / B02 → INT / T
- **R18 →** B01 / B02 → I / INT / T
- **R19 →** B01 / B02 → I / INT / T
- **R20 →** Delivery → DOC
- **R21 →** Only if a service-layer decision is made
- **R22 →** B03 → M / T
- **R23 →** Automated verification → T

Do not rewrite requirements or behaviour contracts here. `Planning.md` remains authoritative.

---

# 3. VERIFICATION OVERVIEW

## Manual verification

### M01 — Add a valid board game
- **Behaviour / requirement:** B01 / R3 / R6
- **Purpose:** Demonstrate successful creation, relevant BGG enrichment, and the defined creation response.
- **Status:** Planned

### M02 — Retrieve board games
- **Behaviour / requirement:** B02 / R2
- **Purpose:** Demonstrate retrieval of locally persisted games through GET.
- **Status:** Planned

### M03 — Discover games by player count
- **Behaviour / requirement:** B03 / R2 / R22
- **Purpose:** Demonstrate the finalized player-count query behaviour; `players=1` is the primary solo-discovery use case.
- **Status:** Planned

### M04 — Reject invalid input
- **Behaviour / requirement:** B04 / R4 / R5
- **Purpose:** Demonstrate the defined validation/error response and absence of invalid persisted state.
- **Status:** Planned

## Integration evidence

### INT01 — BoardGameGeek integration
- **Behaviour / requirement:** B01 / R3 / R7 / R8 / R9
- **Purpose:** Prove the selected BGG operation, relevant data mapping, and external boundary behaviour.
- **Status:** Planned

### INT02 — SQL persistence
- **Behaviour / requirement:** B01 / B02 / R17 / R18 / R19
- **Purpose:** Prove that resources are persisted and retrieved through EF Core and the SQL database.
- **Status:** Planned

### INT03 — External/database failure handling
- **Behaviour / requirement:** B05 / R5 / R11 / R19
- **Purpose:** Prove defined behaviour when a relevant external or database boundary fails. Use controlled failure evidence where manual reproduction is not reliable.
- **Status:** Planned

## Implementation inspection

### I01 — Async I/O path
- **Behaviour / requirement:** B01 / B02 / B05 / R7 / R8 / R9 / R19
- **Purpose:** Verify asynchronous, non-blocking HTTP/database operations and absence of synchronous blocking calls in async methods.
- **Status:** Planned

### I02 — Controller responsibility
- **Behaviour / requirement:** B01 / B02 / B03 / B04 / R13
- **Purpose:** Verify that Controllers own the HTTP boundary without unnecessary domain/persistence responsibility.
- **Status:** Planned

## Documentation / delivery

### DOC01 — README and database setup
- **Requirement:** R14 / R20
- **Purpose:** Verify API description, run steps, testing instructions, and local SQL setup documentation.
- **Status:** In progress — documentation is aligned with the migration-based setup; final verification remains.

### DOC02 — Repository and delivery
- **Requirement:** R12 / R13 / R16
- **Purpose:** Verify required repository contents and GitHub/Canvas delivery.
- **Status:** Planned

---

# 4. AUTOMATED TEST DESIGN

The following tests are the planned automated set. They can be implemented as API/integration tests rather than tests of internal implementation details.

## T01 — Add valid game and persist local resource

- **Behaviour:** B01
- **Requirements:** R1, R3, R6, R17, R18, R19
- **Level:** API / Integration
- **Dependency strategy:** Controlled BGG boundary + test SQLite database

### Concrete cases

- **T01.1 — Valid game creates and persists**
  - Submit a valid BGG ID.
  - Expect `201 Created`.
  - Verify `Location` points to the created resource.
  - Verify the returned game contains the expected mapped data.
  - Verify the game and its player-count recommendations are persisted.
  - Verify BGG is called once.

T01 proves the main create → enrich → persist flow.

## T02 — Retrieve stored games

- **Behaviour:** B02
- **Requirements:** R1, R2, R17, R18, R19
- **Level:** API / Integration
- **Dependency strategy:** Controlled SQLite database

### Concrete cases

- **T02.1 — Collection with stored games**
  - Seed multiple resources.
  - Expect `200 OK` and all stored resources.

- **T02.2 — Empty collection**
  - Start with an empty database.
  - Expect `200 OK` and an empty collection, not `404`.

- **T02.3 — Single resource with recommendations**
  - Seed a game with player-count recommendations.
  - Expect `200 OK` with the game and its recommendation data.

- **T02.4 — Missing resource**
  - Request an ID that does not exist.
  - Expect `404 Not Found`.

## T03 — Discover games by player count

- **Behaviour:** B03
- **Requirements:** R2, R10, R22
- **Level:** API / Integration
- **Dependency strategy:** Controlled SQLite database

### Concrete cases

- **T03.1 — One-player discovery**
  - `players=1` returns only games having a `1` recommendation bucket.
  - This is the primary solo-discovery use case.

- **T03.2 — Other fixed player count**
  - `players=2` returns only games having a `2` recommendation bucket.

- **T03.3 — Open-ended BGG bucket**
  - `players=4+` is accepted and matches the stored `4+` category.

- **T03.4 — No matching games**
  - A valid player-count query with no matches returns `200 OK` and an empty collection.

- **T03.5 — Invalid query value**
  - Zero, negative, and non-numeric player-count values are rejected with `400 Bad Request`.

The API uses the stored BGG recommendation category; it does not claim that the signal is an objective quality rating.

## T04 — Reject invalid POST input

- **Behaviour:** B04
- **Requirements:** R4, R5, R10, R11
- **Level:** API / Integration
- **Dependency strategy:** Controlled SQLite database + controlled BGG boundary

### Concrete cases

- **T04.1 — Zero BGG ID**
  - Expect `400 Bad Request` with a validation problem response identifying `BggId`.
  - BGG must not be called.
  - No resource is persisted.

- **T04.2 — Negative BGG ID**
  - Expect `400 Bad Request`.
  - BGG must not be called.
  - No resource is persisted.

- **T04.3 — Null JSON request body**
  - Send a JSON `null` request body with the JSON content type.
  - Expect `400 Bad Request`.
  - No BGG call is made.

- **T04.4 — Invalid resource ID on GET**
  - A non-positive route ID returns `400 Bad Request`.

## T05 — Handle external dependency failure

- **Behaviour:** B01 / B05
- **Requirements:** R5, R8, R11
- **Level:** API / Integration
- **Dependency strategy:** Controlled BGG boundary

### Concrete cases

- **T05.1 — BGG transport failure**
  - Simulate an HTTP failure at the BGG boundary.
  - Expect the defined external-dependency response (`502 Bad Gateway` in the current contract).
  - No incomplete resource is persisted.

- **T05.2 — BGG resource not found**
  - Simulate a missing BGG game.
  - Expect `404 Not Found`.
  - No resource is persisted.

## T06 — Handle persistence failure

- **Behaviour:** B01 / B02 / B05
- **Requirements:** R8, R11, R19
- **Level:** API / Integration
- **Dependency strategy:** Controlled database failure

### Concrete cases

- **T06.1 — GET database failure**
  - Fail the database boundary during collection retrieval.
  - Expect `500 Internal Server Error`.

- **T06.2 — POST database failure**
  - Fail the database boundary during creation.
  - Expect `500 Internal Server Error` rather than a false success.

## T07 — Duplicate game handling

- **Behaviour:** B01 / B04
- **Requirements:** R4, R5, R11
- **Level:** API / Integration
- **Dependency strategy:** Controlled SQLite database + controlled BGG boundary

### Concrete cases

- **T07.1 — Duplicate POST**
  - Create a valid game once, then submit the same BGG ID again.
  - Expect `409 Conflict`.
  - The second request must not call BGG again.
  - Only one local game and its recommendation records remain persisted.

## Additional external-boundary tests

These tests support INT01 and are separate from the seven top-level behaviour areas because a fake BGG client in T01–T07 cannot prove that the real XML client maps source data correctly.

- **INT01.1 — Map core BGG XML data**
  - Verify ID, primary name, player range, average rating, and rating count.

- **INT01.2 — Map player-count poll categories**
  - Verify categories such as `1` and `4+` and all three vote counts.

- **INT01.3 — BGG HTTP status handling**
  - `404` maps to no game; another unsuccessful response becomes the defined external failure.

- **INT01.4 — BGG request construction**
  - Verify the client requests the intended `thing` endpoint with `stats=1` and sends the configured bearer token when one is configured.

- **INT01.5 — Malformed BGG XML**
  - A successful HTTP response containing malformed XML is treated as an external dependency failure and surfaced as the defined `502 Bad Gateway` response at the API boundary.

These tests should remain deterministic and should not call the live BGG service.

## Relevant test dimensions

Consider only categories that reveal meaningful failures:

- [x] Happy path
- [x] Boundary values
- [x] Equivalence partitions
- [x] Null / empty
- [x] Invalid input
- [x] Missing / not found
- [x] Duplicate
- [x] Dependency failure
- [x] Data/type variation

Do not create a separate test for every checked category. Reuse existing tests when they already provide the needed evidence.

---

# 5. TEST DESIGN REVIEW

Before an automated test is considered ready:

- [x] Referenced behaviour exists in `Planning.md`.
- [x] Requirement(s) are known.
- [x] Scenario and input are concrete.
- [x] Observation point is clear.
- [x] Oracle is clear.
- [x] Test level matches the boundary being proven.
- [x] Dependency strategy is deliberate.
- [x] Relevant edge cases have been considered.
- [x] HTTP, validation, persistence, and external-failure behaviour are covered where relevant.
- [x] A subtly incorrect implementation would be caught.
- [x] The test does not invent a requirement or behaviour.

If a test is difficult to specify, revisit the behaviour contract, observation boundary, responsibility, dependency boundary, or oracle before adding complexity.

---

# 6. NON-AUTOMATED EVIDENCE

Record only evidence that automated tests do not adequately establish.

## Evidence rationale

- **Implementation inspection:** I01–I02 establish internal properties such as asynchronous/non-blocking I/O and responsibility boundaries that API output alone cannot prove.
- **Manual observation:** M01–M04 establish the externally demonstrated endpoint behaviour required by the assignment.
- **Documentation / delivery:** DOC01–DOC02 establish repository, README, database setup, and submission requirements.

---

# 7. IMPLEMENTATION READINESS

This is the hand-off gate from planning into implementation.

The entire test suite does not need to exist as code yet, but the intended verification must be designed sufficiently to prevent invention during implementation.

## Planning readiness

- [ ] `Planning.md` is complete enough to implement from.
- [x] Project direction and core scope are clear.
- [x] Remaining BGG data and player-count signal decisions are recorded.
- [x] Core API routes and required response status behaviour are defined.
- [x] Core domain fields and current validation rules are defined.
- [x] Relevant external and database failure status behaviour is defined.
- [x] SQL persistence and EF Core direction are decided.
- [x] No additional visualization is required to begin the current implementation batch.

## Verification readiness

- [x] Important requirements have evidence coverage.
- [x] Important behaviours have verification methods.
- [x] Required manual, integration, and inspection evidence is identified.
- [x] Planned automated tests have sufficient design information.
- [x] No verification item requires inventing an unrecorded requirement or behaviour.

## Implementation hand-off

- [ ] Implementation strategy is chosen.
- [ ] The project can begin without designing missing scope just to satisfy the first implementation step.

When this gate passes, move to implementation.

---

# 8. IMPLEMENTATION STRATEGY

Choose how the planned behaviours and verification will be turned into working code. This choice does not change requirements, behaviour contracts, or verification design.

- **Test-first batch:** Create the planned test files first, establish meaningful failures, then implement the corresponding behaviours.
- **Incremental TDD:** Implement one already-planned behaviour/test at a time through RED → GREEN → REFACTOR → VERIFY.
- **Hybrid:** Create a limited batch of tests and implement the batch incrementally.

- **Chosen strategy:** Test-first batch
- **Why:** The MVP behaviours have been identified and the project is now using a batch of concrete tests to expose remaining implementation gaps before the corresponding production code is considered complete.

---

## Source-of-truth rule

- `Planning.md` → requirements, direction, scope, behaviours, acceptance contracts, domain rules, API/design decisions, and technical baseline.
- `TestPlan.md` → verification strategy, coverage, automated test design, non-automated evidence, and implementation readiness.
- Concrete test file → executable automated test behaviour.
- Todoist → work status and task completion when Todoist is used.

Do not duplicate behaviour contracts or requirements between the two planning files. Reference the owner instead.
