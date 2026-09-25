# BoardGame Library API – PLANNING

## How to use this file

- Follow the manual's main flow, but only complete sections that are relevant.
- Keep source facts, assumptions, and your own design choices separate.
- Do not pre-design future implementation just because it is possible.
- When new information changes an earlier decision, revisit the lowest section that actually needs to change.
- `Planning.md` is the project's planning truth: keep current requirements, decisions, scope, and relevant design/behaviour information here. Task completion is progress tracking, not a second source of truth.
- When the assignment leaves the project direction open, choose the direction before finalizing the direction-dependent planning sections.

---

# 0. PROJECT CONTEXT

- **Goal / desired outcome:**

  Build a small REST API for a personal board-game library that stores a local collection in SQL and enriches board-game resources with relevant BoardGameGeek information. The API should make it easier to discover games suitable for solo play while demonstrating meaningful backend/API development.

- **Problem / need:**

  BoardGameGeek provides useful board-game metadata and player-count information, but information relevant to solo play is not presented as a dedicated, locally manageable discovery workflow. The project will combine relevant external data with a local collection so that owned games can be queried and filtered according to information useful for solo discovery.

- **User / recipient:**

  Primarily the developer/user of the personal board-game library.

- **Stakeholder(s):**

  Evaluator.

- **Decision owner:**

  Course/assignment owner when an important assignment ambiguity cannot otherwise be resolved.

- **Deliverables:**

  A GitHub repository.

  The repository should contain:

  - `Model/` with the domain/data model;
  - Controller(s) with GET and POST endpoints;
  - the local SQL persistence required by the selected direction;
  - `README.md` describing the API, how to run it, and how to test the endpoints.

  The README should also document the relevant database setup because SQL persistence is part of the selected project.

- **Deadline / timeframe:**

  27 September 2026.

- **Non-negotiable constraints:**

  - Use C#.
  - Build a REST API using Controllers.
  - Define a meaningful domain/data model.
  - Provide GET endpoint(s).
  - Provide a POST endpoint.
  - Validate POST input.
  - Use appropriate HTTP responses/status codes.
  - Use asynchronous, non-blocking handling where required.
  - Use asynchronous database I/O.
  - Include the required README documentation.
  - Deliver the project through a GitHub repository linked in Canvas.

- **Initial unknowns:**

  - The exact final representation of BGG player-count buckets such as `4+` in the local model.
  - Which filtering/sorting features should be exposed beyond the core `players` query.
  - The concrete validation rules for POST input.
  - The exact error response strategy for external BGG and database failures.
  - Whether a service layer is justified by a concrete responsibility.

---

# 1. ORIENT

## What the assignment means in your own words

I need to build a small REST API around a meaningful domain model. The API must expose GET and POST operations through Controllers. GET must retrieve data, while POST must create a new resource and validate its input. The API should use appropriate HTTP behaviour, clear error handling, and asynchronous, non-blocking handling.

The selected direction extends the basic assignment with a real use case: a local board-game library enriched with relevant BoardGameGeek information. SQL persistence and Entity Framework Core are deliberately selected because the project benefits from persistent local data and database querying. External HTTP access provides a meaningful asynchronous I/O boundary.

The assignment's optional service layer, advanced GET features, and xUnit project remain separate choices and should only be added where they provide a concrete benefit.

## Existing project, if any

- **Existing functionality:**

  New project.

- **Existing contracts to preserve:**

  New project; no existing application contracts have been identified.

## Unknown terms / technologies

No specific terminology or technology has been identified as an actual blocker from the assignment alone.

Add a concrete entry here only when an unfamiliar framework API, BGG mechanism, database mechanism, tool, or other mechanism must be understood before continuing.

> Do not list technologies simply because they may be encountered later.

## Preliminary risks

- The project could become an unnecessarily large BGG integration instead of a focused REST API.
- External BGG data could provide more information than the local domain actually needs.
- Solo suitability could be represented too simplistically and confuse general player-count data with actual solo quality.
- SQL or EF Core could lead to unnecessary architecture if the persistence responsibility is kept too broad.
- External API failures or limitations could make POST behaviour unreliable if not handled explicitly.
- Optional filtering, service-layer work, or automated testing could expand the project beyond the useful scope.
- HTTP and error behaviour could remain vague unless defined before implementation.

---

# 2. PROJECT DIRECTION

The project direction has been selected.

## Assignment freedom

The assignment fixes the technical foundation but leaves the concrete project domain open.

The project can choose:

- the API domain/resource;
- the model and its meaningful properties;
- whether GET retrieves a collection, a single resource, or both;
- whether filtering, sorting, and/or pagination are useful;
- whether SQL persistence is used;
- whether a service layer is justified;
- whether an xUnit test project is added;
- small domain-specific extensions that remain appropriate to the assignment.

The selected direction must still satisfy the assignment's required Controller, GET, POST, validation, HTTP, asynchronous, documentation, testing, and delivery requirements.

## Portfolio gap

The portfolio already contains substantial C#/.NET, backend, API, and asynchronous programming work. This project therefore should not be another generic CRUD exercise.

The selected direction adds a more distinctive backend problem through:

- external API integration;
- asynchronous HTTP I/O;
- SQL persistence;
- Entity Framework Core;
- transformation of external data into a local domain model;
- meaningful filtering and discovery behaviour.

The aim is to add new technical signal without adding unnecessary complexity.

## Candidate directions

The following directions were considered as realistic fits for the assignment:

- **Personal Board Game Library & Solo Discovery API:** Local library enriched with BoardGameGeek information. Adds external integration, async I/O, persistence, domain modelling, validation, and meaningful querying.
- **Project / Work Tracker API:** Projects, tasks, status, priorities, and deadlines. Adds business-oriented domain modelling and state/validation rules.
- **Training Session API:** Training sessions with activity, duration, date, and measurements. Adds date/time handling, validation, filtering, and domain-specific data modelling.

The candidates were considered as project directions rather than implementation designs.

## Chosen direction

**Personal Board Game Library & Solo Discovery API with SQL persistence**

The API will maintain a local library of board games and enrich local resources with relevant BoardGameGeek information. The project will focus on making it easier to discover which owned games are suitable or promising for solo play.

## Why this direction

This direction fits the assignment naturally while providing a useful technical problem.

The local database gives the project persistent user-owned data that can be queried without depending on BGG for every GET request.

The external BGG integration creates a genuine asynchronous I/O boundary for adding or enriching games.

The domain also provides meaningful reasons for validation, filtering, sorting, and data modelling rather than adding those features merely because they are technically possible.

The project remains intentionally focused: store the relevant local representation, retrieve useful BGG data, expose it through a REST API, and support useful discovery queries.

## Portfolio role

The project should demonstrate:

- REST API design;
- asynchronous external HTTP integration;
- asynchronous SQL/database access;
- Entity Framework Core;
- external-data-to-local-domain transformation;
- validation and HTTP error handling;
- meaningful filtering/query behaviour;
- practical backend/domain modelling.

Its primary portfolio role is **external integration + persistent backend API + domain modelling**, complementing rather than duplicating existing C#/.NET and asynchronous work.

---

# 3. REQUIREMENTS

Record requirements from the assignment, approved clarifications, or project decisions.

## Mandatory requirements

- **R1 — Meaningful domain model** · Functional · MUST · Assignment – Part 1  
  The project shall define a meaningful domain/data model with meaningful properties.

- **R2 — GET endpoint** · Functional · MUST · Assignment – Part 2  
  The API shall expose a GET endpoint that retrieves data for the selected resource.

- **R3 — POST endpoint** · Functional · MUST · Assignment – Part 2  
  The API shall expose a POST endpoint that creates a new resource.

- **R4 — POST validation** · Functional · MUST · Assignment – Part 2  
  The POST operation shall validate input.

- **R5 — Clear invalid-input feedback** · Functional · SHOULD · Assignment – Part 2 / Quality requirements  
  The API shall provide clear error feedback for invalid input.

- **R6 — Successful creation response** · Quality · MUST · Assignment – Part 2  
  Successful resource creation shall return an appropriate HTTP status and a useful response that points to the new resource where applicable.

- **R7 — Asynchronous endpoint handling** · Quality · SHOULD · Assignment – Async code guidance  
  Endpoints should be asynchronous and non-blocking.

- **R8 — Non-blocking I/O** · Technical · MUST · Assignment – Async code guidance  
  I/O must not use synchronous blocking calls inside asynchronous methods.

- **R9 — Task-based asynchronous design** · Quality · SHOULD · Assignment – Async code guidance  
  Task-based method design should be used and asynchronous behaviour should propagate through the call chain where applicable.

- **R10 — HTTP semantics** · Quality · SHOULD · Assignment – Quality requirements  
  HTTP semantics should be handled correctly, including the distinction between read and create operations.

- **R11 — Consistent error handling** · Quality · SHOULD · Assignment – Quality requirements  
  The API should use a consistent error-handling strategy.

- **R12 — Model directory** · Delivery · MUST · Assignment – Delivery  
  The repository shall contain a `Model/` directory with the domain model.

- **R13 — Controllers** · Delivery · MUST · Assignment – Delivery  
  The repository shall contain Controller(s) with GET and POST endpoints.

- **R14 — README** · Delivery · MUST · Assignment – Delivery  
  The README shall describe the API, how to run it, and how to test the endpoints.

- **R15 — Manual endpoint verification** · Verification · MUST · Assignment – Testing / Verification  
  The endpoints shall be manually testable using Swagger, cURL, Postman, or another appropriate method.

- **R16 — GitHub / Canvas delivery** · Delivery · MUST · Assignment – Delivery  
  The project shall be submitted as a GitHub repository link in Canvas.

## Selected optional requirements

- **R17 — SQL persistence** · Technical · MUST · Assignment – Optional SQL support + project decision  
  The project will use SQL persistence for the local board-game library.

- **R18 — Entity Framework Core** · Technical · MUST · Assignment – Optional SQL support + project decision  
  Entity Framework Core will be used as the selected data-access framework for the SQL database.

- **R19 — Async database access** · Technical · MUST · Assignment – Optional SQL support + project decision  
  Database access shall remain asynchronous and non-blocking.

- **R20 — SQL documentation** · Delivery · MUST · Assignment – Optional SQL support + project decision  
  The README shall document the relevant local database setup and run steps.

## Remaining optional requirements

- **R21 — Service layer** · Design · MAY · Assignment – Optional service layer  
  A service layer may be used if a concrete responsibility justifies it.

- **R22 — GET extensions** · Functional · MAY · Assignment – Optional paging/filtering  
  Filtering, sorting, and/or pagination may be added when meaningful for the selected resource.

- **R23 — Automated API tests** · Verification · MUST · Assignment – Optional xUnit project + project decision  
  This project will include a separate xUnit test project for automated API verification.

> Use `MUST / SHOULD / MAY` only when the source or an explicit project decision establishes that strength.

## Requirement quality check

For important requirements:

- [ ] Clear enough to understand without guessing.
- [ ] Consistent with the other requirements.
- [ ] Complete enough to describe the important input/result/constraint.
- [ ] Verifiable later.
- [ ] Feasible within the assignment and chosen technology.
- [ ] Source is known.

---

# 4. CLARIFICATIONS / ASSUMPTIONS / OPEN QUESTIONS

Use this section only for genuinely unresolved project questions.

- **A1 — Player-count bucket representation** · Open question  
  The BGG source can contain values such as `4+`; the local representation must preserve the meaning without reducing it to a misleading fixed integer.  
  **Why it matters:** Affects the concrete C#/SQL representation of player-count recommendations.

- **A2 — GET extensions** · Open question  
  The core `players` query has been selected, but additional filtering/sorting should only be added if it provides useful behaviour.  
  **Why it matters:** Determines final GET scope and verification effort.

- **A3 — POST validation rules** · Open question  
  The exact input-validation rules have not yet been finalized.  
  **Why it matters:** Determines B04 and the POST error contract.

- **A4 — Error response strategy** · Open question  
  The relevant failure categories are known, but the final error-response shape and status mapping still need to be defined.  
  **Why it matters:** Determines B05 and the API error contract.

- **A5 — Service layer** · Open question  
  A service layer remains optional until a concrete responsibility justifies it.  
  **Why it matters:** Avoids adding architecture without a real separation need.

- **A6 — Project type** · Assumption · Confirmed  
  This is a new project with no existing behaviour or contracts to preserve.

> Do not silently choose interpretations when an unresolved decision can materially change the project. Resolve important questions before the affected behaviour is finalized.

---

# 5. RESEARCH / SPIKES

Research is conditional.

Add a research block only when a concrete unknown can change the current project decision or implementation.

Likely research triggers include:

- understanding the required BGG API operation and response data;
- determining relevant BGG player-count/solo signals;
- confirming the appropriate EF Core setup for the selected SQL database;
- resolving an async HTTP or database-access question;
- testing a small technical integration when documentation alone is insufficient.

## Research record

- **Question:** [What must be learned?]
- **Source / experiment:** [Official documentation, small trial, or spike]
- **Finding:** [What was learned?]
- **Decision:** [What will be done now?]

---

# 6. SCOPE / SUCCESS / DEFINITION OF DONE

## Implementation boundaries

The project will focus on:

- local persistence of a deliberately small board-game domain model;
- relevant BoardGameGeek data retrieval;
- transformation into the local model;
- Controller-based API behaviour;
- validation;
- meaningful GET querying;
- asynchronous HTTP and database I/O.

The project will not attempt to reproduce the full BoardGameGeek data model or build a general-purpose BGG client.

## In scope

- Board-game domain/data model.
- Local SQL database.
- Entity Framework Core.
- Controller-based GET endpoint(s).
- Controller-based POST endpoint.
- Validation of POST input.
- Retrieval of relevant BGG information.
- Local persistence of the selected BGG-derived data.
- Relevant solo/player-count information.
- Meaningful GET filtering and/or sorting where justified.
- Appropriate HTTP responses and error handling.
- Asynchronous external HTTP handling.
- Asynchronous database access.
- README documentation.
- Manual endpoint verification.
- GitHub/Canvas delivery.

## Out of scope

Initially:

- Full reproduction of BGG's domain/data model.
- Importing an entire BGG collection through a complex synchronization system.
- Authentication/authorization.
- Background synchronization jobs.
- Caching infrastructure.
- Redis or other additional infrastructure.
- Service/repository layers without a concrete responsibility.
- Advanced pagination or query features without a concrete need.
- xUnit testing unless deliberately selected.

Optional work must not displace completion of the core API.

## Project success

The project is successful when the selected board-game library API provides a meaningful local model, persists data through SQL, can create and retrieve resources through Controllers, uses relevant BGG information to support the chosen discovery purpose, handles validation and HTTP errors appropriately, performs applicable I/O asynchronously, and satisfies the required documentation, verification, and delivery requirements.

## Definition of Done

- [ ] Required API behaviour is implemented and verified.
- [ ] SQL persistence works through the selected EF Core setup.
- [ ] Relevant BGG integration works and failure behaviour is handled.
- [ ] Required documentation and delivery are complete.
- [ ] Relevant evidence is complete.
- [ ] Build and run work as required by the assignment.
- [ ] Manual endpoint verification has been performed.
- [ ] GitHub repository has been submitted through Canvas.

---

# 7. RISK / DEPENDENCIES

Use this section only where a risk or dependency can affect scope, design, testing, or delivery.

## Risks

- **External BGG dependency fails or changes** — Could prevent game creation/enrichment. Response: define explicit external-failure behaviour and keep the local model independent of the full external response.
- **Solo suitability is overstated** — Could make the application present weak signals as factual conclusions. Response: distinguish raw BGG player-count signals from project-derived or user-provided solo information.
- **Domain grows too large** — Could turn the assignment into a full BGG client. Response: store only the data required for the selected use case.
- **SQL architecture becomes excessive** — Could consume time without increasing project value. Response: begin with the simplest EF Core setup that satisfies the persistence responsibility.
- **GET becomes unnecessarily complex** — Could expand scope and testing. Response: add only queries that serve the selected discovery use case.

## Dependencies

- **BoardGameGeek API** — Why needed: External board-game information. · What can fail: Network, rate limiting, unavailable data, or response differences. · Must be known: Required operation and data fields.
- **EF Core / SQL database** — Why needed: Persistent local library. · What can fail: Configuration, schema, connection, or database I/O. · Must be known: Basic async persistence flow.
- **ASP.NET Core Controllers** — Why needed: Required REST API structure. · What can fail: Routing/configuration/API misunderstanding. · Must be known: Controller and routing model.
- **Swagger or another API client** — Why needed: Manual endpoint verification. · What can fail: Incorrect requests or unclear responses. · Must be known: Endpoint contracts.

---

# 8. WORK MAP / BEHAVIOURS

Map the important behaviours the project is expected to need. The exact breakdown can be refined after research and API-contract decisions.

- **B01 — Add a board game** · Planned  
  **Behaviour:** Accept a valid game identifier and relevant user-provided data, retrieve the required external game information, and create the corresponding local resource.  
  **Importance / risk:** High; crosses validation, HTTP, external I/O, domain mapping, and database persistence.  
  **Dependencies:** BGG API, model, validation, EF Core.

- **B02 — Retrieve board games** · Planned  
  **Behaviour:** GET retrieves the stored board-game collection or an individual resource according to the finalized API contract.  
  **Importance / risk:** High; establishes the main read API.  
  **Dependencies:** Domain model, database, GET contract.

- **B03 — Discover games by player count** · Planned  
  **Behaviour:** GET supports meaningful retrieval/filtering based on selected player-count and solo-related data.  
  **Importance / risk:** High; this is the distinctive domain purpose of the project.  
  **Dependencies:** Selected BGG data, local model, query design.

- **B04 — Reject invalid requests** · Planned  
  **Behaviour:** Invalid POST input is rejected with clear validation feedback and an appropriate HTTP response.  
  **Importance / risk:** High; directly required by the assignment.  
  **Dependencies:** Validation rules and error contract.

- **B05 — Handle relevant failures** · Planned  
  **Behaviour:** Relevant external, persistence, and API failures are handled consistently according to the finalized error strategy.  
  **Importance / risk:** Medium to high.  
  **Dependencies:** Actual boundaries and error decisions.

Add further behaviours only when a concrete project requirement or selected extension justifies them.

---

# 9. BEHAVIOUR CONTRACTS

> Define the acceptance contract for each planned behaviour before implementation. `TestPlan.md` references these contracts rather than rewriting them.

## B01 — Add a board game

- **Requirement(s):** R1, R3, R4, R6, R7, R8, R9, R10, R13, R17, R18, R19.
- **Acceptance criterion:** A valid request creates a local board-game resource using the required external BGG information and returns the appropriate creation response.
- **Precondition / input:** A valid request containing the required game identifier and any required personal/library information.
- **Action:** Submit the POST request and perform the required external and database operations.
- **Expected result:** The relevant external data is transformed into the local model, the resource is persisted, and the API returns the appropriate successful HTTP response.
- **Observation boundary:** HTTP response and persisted database state.
- **Relevant edge cases:** Invalid identifier, unavailable external resource, malformed external response, duplicate local resource, database failure.

## B02 — Retrieve board games

- **Requirement(s):** R1, R2, R7, R8, R9, R10, R13.
- **Acceptance criterion:** A valid GET request returns the stored board-game resource(s) according to the finalized API contract.
- **Precondition / input:** Database contains zero or more valid board-game resources.
- **Action:** Submit the relevant GET request.
- **Expected result:** The API returns the expected resource or collection with an appropriate HTTP response.
- **Observation boundary:** HTTP response and returned resource data.
- **Relevant edge cases:** Empty collection, missing resource, database failure.

## B03 — Discover games by player count

- **Requirement(s):** R2, R10, and R22 if GET extensions are retained.
- **Acceptance criterion:** A valid `players` query returns games that have BoardGameGeek community recommendation data for the requested player count and exposes the corresponding recommendation values.
- **Precondition / input:** Local resources contain one or more `PlayerCountRecommendation` records.
- **Action:** Submit `GET /api/games?players={n}`.
- **Expected result:** Returned games have recommendation data for the requested player-count bucket. `players=1` provides the project's primary solo-discovery use case.
- **Observation boundary:** HTTP response and returned collection.
- **Relevant edge cases:** No matching games, incomplete source data, `4+` buckets, and invalid player-count query values.

> The community recommendation signal is distinct from the game's official `MinPlayers` / `MaxPlayers` range. The API must not convert the raw BGG data into an objective proprietary "solo rating" in the MVP.

## B04 — Reject invalid requests

- **Requirement(s):** R4, R5, R10, R11.
- **Acceptance criterion:** Invalid POST input is rejected without creating an invalid local resource.
- **Precondition / input:** Request violates one or more defined validation rules.
- **Action:** Submit the POST request.
- **Expected result:** The API returns the defined validation/error response and does not persist the invalid resource.
- **Observation boundary:** HTTP response and database state.
- **Relevant edge cases:** Missing fields, invalid values, invalid identifier, duplicate where applicable.

## B05 — Handle relevant failures

- **Requirement(s):** R5, R8, R10, R11.
- **Acceptance criterion:** Relevant external and persistence failures produce defined and understandable API responses.
- **Precondition / input:** A relevant dependency or boundary failure occurs.
- **Action:** Execute the affected API operation.
- **Expected result:** The failure is handled according to the finalized error strategy without silently producing invalid state.
- **Observation boundary:** HTTP response and resulting state.
- **Relevant edge cases:** External service unavailable, database failure, timeout, unavailable resource.

> Do not create implementation details inside these contracts. Define the observable behaviour and let the design section determine how it is achieved.

---

# 10. DATA / STATE / RULES

> Define only the data, rules, and state that the selected behaviours actually need.

## Data / model

The local model contains only information required for the selected board-game library and player-count discovery use case.

### BoardGame

- **Identity:** `BggId` is the primary identity of the local board-game resource.
- **Properties:** `BggId`, `Title`, `MinPlayers`, `MaxPlayers`, `BggAverageRating`, and `BggRatingCount`.
- **Value constraints:** `BggId` must identify a retrievable BGG game; numeric values must remain valid for their source meaning.
- **Null / empty:** Required identity/title data must be available; optional source data must not be replaced with invented values.

### PlayerCountRecommendation

- **Identity:** Belongs to one `BoardGame` and represents one BGG player-count recommendation bucket.
- **Properties:** Player-count bucket, `BestVotes`, `RecommendedVotes`, and `NotRecommendedVotes`.
- **Player-count representation:** Must preserve source values such as `1`, `2`, `3`, `4`, and `4+` without implying that an open-ended source bucket has a finite upper bound.
- **Validation responsibility:** External source values are mapped into the local model before persistence; request validation occurs before resource creation.

## State

### Add game

- **Before:** The submitted BGG game does not yet exist as the local resource.
- **Action:** Validate input, obtain required BGG data, map the BoardGame and player-count recommendation data, and persist the local resource.
- **After:** The valid resource and its relevant player-count recommendation records exist in the local database.

### Read games

- **Before:** The local database contains zero or more valid resources.
- **Action:** Query the local data.
- **After:** No persistent state is changed.

### Invariants

- A persisted resource must satisfy the defined domain/data rules.
- GET operations are read-only.
- Invalid input must not create invalid persisted state.
- The local model must not require the complete external BGG data model.

## Relevant edge cases

- [ ] Boundary values
- [ ] Null / empty
- [ ] Invalid input
- [ ] Missing / not found
- [ ] Duplicate
- [ ] State transition
- [x] Dependency failure
- [x] Data/type variation

Only checked categories require explicit handling. Add or remove categories as the final design becomes clearer.

### Rules / edge cases

- The project must distinguish external BGG data from locally stored user/library data.
- Solo-related signals must not automatically be presented as an objective measure of solo quality unless the data source actually supports that interpretation.
- External data that is unavailable or incomplete must have defined behaviour before it is persisted.
- Duplicate handling must be defined for repeated attempts to add the same game.

---

# 11. DESIGN / API / BOUNDARIES

## Responsibilities

### Controller

- **Owner of:** HTTP request/response handling and API-level coordination.
- **Why:** Controllers are required by the assignment and should remain focused on the HTTP boundary.
- **Notes:** Do not move domain or persistence responsibilities into the Controller unless a concrete reason requires it.

### Domain/model

- **Owner of:** Local board-game representation and relevant domain/data rules.
- **Why:** The project needs a meaningful local model rather than mirroring the entire external service.
- **Notes:** Keep the model deliberately small.

### Persistence

- **Owner of:** Storing and querying the local board-game library.
- **Why:** SQL persistence is a selected project requirement.
- **Notes:** Use EF Core with asynchronous database access.

### External integration

- **Owner of:** Retrieving the relevant information from BoardGameGeek.
- **Why:** External data is part of the selected project direction.
- **Notes:** Keep the external representation separate from the local domain model.

A separate service layer remains optional until a concrete responsibility makes it useful.

## Public API – when relevant

### POST /api/games

- **Parameters:** `bggId` plus only the request data that the finalized library model actually requires.
- **Return type / HTTP result:** `201 Created` with the created resource and a location pointing to the created resource.
- **Errors:** Validation failure, duplicate resource, unavailable BGG resource, BGG dependency failure, and persistence failure according to the finalized error contract.
- **Side effects:** External BGG retrieval and local database creation.
- **Contract:** A valid request creates one valid local `BoardGame` resource with its relevant player-count recommendation data.

### GET /api/games

- **Parameters:** Optional `players` query parameter. Additional filters/sorting are not part of the locked contract yet.
- **Return type / HTTP result:** `200 OK` with the locally stored board-game collection.
- **Errors:** Invalid query value where applicable and persistence failure according to the finalized error contract.
- **Side effects:** None.
- **Contract:** Returns current local library data without changing persistent state.

### GET /api/games/{bggId}

- **Parameters:** `bggId` identifying the local board-game resource.
- **Return type / HTTP result:** `200 OK` with one board-game resource.
- **Errors:** `404 Not Found` when the local resource does not exist; persistence failure according to the finalized error contract.
- **Side effects:** None.
- **Contract:** Returns the requested locally stored board-game resource.

### GET /api/games?players={n}

- **Parameters:** `players`, representing the requested BGG player-count bucket.
- **Return type / HTTP result:** `200 OK` with games having community recommendation data for the requested bucket.
- **Errors:** Invalid query value according to the finalized validation rules and persistence failure according to the finalized error contract.
- **Side effects:** None.
- **Contract:** Uses the stored BGG player-count recommendation data. `players=1` is the primary solo-discovery use case; it is not a claim that the returned games have an objectively measured solo quality.

## Boundaries

### HTTP / Controller

- **Input / output:** HTTP request / HTTP response.
- **Important failure:** Validation and API-level errors.
- **Current strategy:** Define final status mappings with the concrete API contracts.

### BoardGameGeek API

- **Input / output:** BGG game ID / relevant board-game and player-count data.
- **Important failure:** Network failure, unavailable resource, invalid/unexpected response, authorization or service limitation.
- **Current strategy:** Async HTTP access; map only the required source data into the local model and keep the BGG representation behind the external boundary.

### SQL database

- **Input / output:** Local entity data / query results.
- **Important failure:** Connection, configuration, query, or persistence failure.
- **Current strategy:** EF Core with asynchronous database access.

---

# 12. VISUALIZATION / PSEUDOCODE – WHEN NEEDED

A visualization may be useful because the project crosses an HTTP boundary, an external API boundary, and a database boundary.

Create one only if it improves understanding of the flow.

- **Tool:** Flowchart or sequence diagram if needed.
- **Purpose:** Clarify the POST flow from client → Controller → external BGG data → local model → SQL database, and the GET flow from Controller → SQL database → response.
- **Reference:** [File/link/location / `-`]

---

# 13. TRACEABILITY / DECISIONS

## Traceability

Use IDs when they improve navigation.

`Source / requirement → behaviour`

Detailed verification mapping belongs in `TestPlan.md`.

- **R1 →** B01 / B02 / B03 / B04
- **R2 →** B02 / B03
- **R3 →** B01
- **R4 →** B04
- **R5 →** B04 / B05
- **R6 →** B01
- **R7 →** B01 / B02 / B03 / B05 as applicable
- **R8 →** B01 / B02 / B05 where I/O is involved
- **R9 →** B01 / B02 / B05 where async I/O is involved
- **R10 →** B01 / B02 / B03 / B04 / B05
- **R11 →** B04 / B05
- **R12 →** Delivery
- **R13 →** B01 / B02 / B03 / B04
- **R14 →** Delivery
- **R15 →** Verification evidence
- **R16 →** Delivery
- **R17 →** B01 / B02
- **R18 →** B01 / B02
- **R19 →** B01 / B02
- **R20 →** Delivery
- **R21 →** Only if a service-layer decision is made
- **R22 →** B03 if GET extensions are selected
- **R23 →** TestPlan / automated verification

> Delivery and verification requirements do not need a system behaviour. Their evidence belongs in `TestPlan.md`.

## Decisions

### D1 — BoardGameGeek-based board-game library

- **Decision:** Use a personal board-game library enriched with relevant BoardGameGeek data as the project domain.
- **Reason:** Provides a meaningful domain and a concrete user problem while supporting several relevant backend concepts.
- **Consequence:** The project has an external-data boundary and a local domain model.
- **Revisit when:** A fundamental constraint makes the direction infeasible.

### D2 — SQL persistence

- **Decision:** Use SQL persistence as part of the core project.
- **Reason:** The project represents a persistent personal library and benefits from querying and retaining externally retrieved data.
- **Consequence:** EF Core, database setup, migrations/schema work, and asynchronous database access become part of the project.
- **Revisit when:** The chosen database approach becomes disproportionate to the assignment or technically infeasible.

### D3 — Entity Framework Core

- **Decision:** Use Entity Framework Core for SQL data access.
- **Reason:** It is explicitly recommended by the assignment and is appropriate for the selected persistence scope.
- **Consequence:** The database boundary will be implemented through EF Core.
- **Revisit when:** A concrete technical limitation appears.

### D4 — Keep the local model smaller than the external model

- **Decision:** Store only the BGG information needed for the project's local board-game and solo-discovery purpose.
- **Reason:** Avoid turning the assignment into a general BGG data mirror.
- **Consequence:** External-to-local mapping is an explicit part of the design.
- **Revisit when:** A required behaviour needs additional data.

### D5 — Keep optional architecture conditional

- **Decision:** Do not add a service layer, repository layer, caching, background processing, or other architecture without a concrete responsibility.
- **Reason:** Preserve project focus.
- **Consequence:** The implementation remains as simple as the behaviours allow.
- **Revisit when:** A real responsibility or boundary justifies separation.

### D6 — Distinguish solo signals from general game rating

- **Decision:** Treat general BGG rating, player-count signals, and any project-specific solo information as distinct data/concepts.
- **Reason:** Overall game popularity does not by itself establish solo quality.
- **Consequence:** The API should not present a derived solo-quality judgement as objective source data.
- **Revisit when:** The final source data and solo model are defined.

### D7 — Use BGG player-count recommendations as a general relation

- **Decision:** Store `Best`, `Recommended`, and `Not Recommended` votes per BGG player-count bucket rather than creating solo-specific fields.
- **Reason:** The source data is general player-count data; solo is the `1 player` case.
- **Consequence:** The same model supports discovery for one or more players, while `players=1` remains the primary solo use case.
- **Revisit when:** The BGG source format or selected discovery requirements materially change.

### D8 — Use BggId as local resource identity

- **Decision:** Use `BggId` as the primary identity of `BoardGame` rather than introducing a separate local game identifier.
- **Reason:** Each local resource currently represents one unique BGG game.
- **Consequence:** Duplicate detection and the resource route can use the same stable identifier.
- **Revisit when:** The local domain gains multiple distinct resources tied to the same BGG game.

### D9 — Keep the MVP API surface small

- **Decision:** Start with `POST /api/games`, `GET /api/games`, `GET /api/games/{bggId}`, and the `players` query on the collection GET.
- **Reason:** These operations directly support the assignment and the selected use case without unnecessary endpoints.
- **Consequence:** Update/delete, refresh, statistics, and other endpoints remain out of scope unless later justified.
- **Revisit when:** A concrete project requirement creates a real need for another endpoint.

### D10 — Always use xUnit in the project workflow

- **Decision:** Include an xUnit test project even though the assignment makes automated API testing optional.
- **Reason:** Automated testing is a standard part of the project workflow and provides useful executable verification.
- **Consequence:** R23 is treated as a project MUST rather than an unresolved optional feature.
- **Revisit when:** The project workflow itself changes.

---

# 14. ENVIRONMENT / BASELINE

## New project / setup

- **Language / target framework:** C# / ASP.NET Core.
- **Test framework/runner:** xUnit.
- **SDK version:** To be confirmed during environment setup.
- **Database:** Local SQL database using Entity Framework Core.
- **External integration:** BoardGameGeek API.
- **Other required setup:** Controller-based REST API project, EF Core configuration, required planning/documentation files, and GitHub repository.

## Existing project

- **Solution/project:** Not applicable.
- **Build command:** Not applicable.
- **Test command:** Not applicable.

## Baseline

- [ ] Required project structure is ready.
- [ ] ASP.NET Core API project builds.
- [ ] SQL/EF Core setup works.
- [ ] External HTTP dependency can be reached through the chosen integration approach.
- [ ] Required toolchain/setup is understood well enough to continue.

---

## PLANNING READINESS

Before implementation begins:

- [x] Assignment and project purpose are understood.
- [x] Project direction is selected.
- [x] Portfolio role is identified.
- [x] Core requirements are identified and sourced.
- [ ] Direction-dependent open questions are resolved enough for implementation.
- [x] Scope is defined.
- [x] Important behaviours are mapped.
- [x] Behaviour contracts exist for the planned behaviours.
- [x] Core API routes and operation shapes are selected.
- [x] Core domain fields and BGG data contract are selected.
- [x] Required BGG integration boundary is defined.
- [x] SQL persistence and EF Core direction are decided.
- [ ] Final validation and dependency-failure behaviour is defined.
- [ ] Relevant visualization is created if it materially improves understanding.
- [ ] TestPlan contains the verification design needed for implementation.

When this gate passes, Planning is complete enough to hand over to implementation.

---

## NEXT

The next planning decision is to finalize:

1. POST input validation rules.
2. Duplicate-game behaviour.
3. Error-response/status mapping for validation, BGG, and database failures.
4. Whether any GET sorting/filtering beyond `players` is genuinely needed.
5. Whether a service layer has a concrete responsibility.
6. Update affected behaviour contracts and decisions.
7. Update `TestPlan.md` against the finalized contracts.
8. Re-run the Planning Readiness gate.

Do not add architecture or optional features merely because they are available.

---

## Source-of-truth rule

- `Planning.md` → project facts, requirements, project direction, scope, behaviour contracts, domain/rules, design, decisions, and environment.
- `TestPlan.md` → verification strategy, coverage, test design, evidence, and implementation-readiness for testing.
- Concrete test file → executable automated test behaviour.
- Todoist → work status and task completion when Todoist is used.

A completed task does not replace current project truth.

## BGG data contract

Define the smallest BGG dataset that is sufficient for the chosen project direction.

The purpose is to decide what the application actually depends on before designing the database or implementation.

### Import scope

The application will import board-game data from BoardGameGeek.

Store only data that supports one or more of these purposes:

- identifying the game
- displaying useful game information
- describing general BGG popularity/rating
- describing suitability for different player counts
- supporting meaningful API queries and filtering

Do not mirror the BGG API. Data is included because the application needs it, not because BGG exposes it.

### Board game data

- **BGG ID** — external identifier used to identify the game in BGG.
- **Name** — game title.
- **Year Published** — publication year, where available.
- **Min Players** — minimum supported player count.
- **Max Players** — maximum supported player count.
- **Playing Time** — BGG's reported playing-time value.
- **Min Playing Time** — minimum reported playing time.
- **Max Playing Time** — maximum reported playing time.
- **Min Age** — BGG's reported minimum age.
- **Average Rating** — general BGG user rating.
- **Users Rated** — number of BGG users contributing to the rating.
- **Bayes Average** — BGG's Bayesian average.
- **Average Weight** — BGG's reported complexity/weight rating.
- **BGG Rank** — BGG's ranking value, where available.
- **Best With** — BGG's summarized recommended player-count information.
- **Recommended With** — BGG's summarized recommended player-count information.

### Player-count recommendations

Player-count suitability is represented separately from the general board-game data.

For each player-count category returned by BGG, store:

- **Player Count**
- **Best Votes**
- **Recommended Votes**
- **Not Recommended Votes**

The representation must support the player-count categories actually returned by BGG, including categories such as `4+` where applicable.

Do not convert these values into a fabricated "solo rating", "two-player rating", or similar score.

The raw vote categories should remain distinguishable so that the application can later derive its own queries or presentation from the underlying data.

### Important distinctions

**General BGG rating is not a player-count rating.**

`Average Rating` describes the game's overall BGG rating and must not be presented as evidence that the game is good for a particular player count.

**BGG rank is not the same as rating.**

`BGG Rank` is contextual ranking information and is not required to determine player-count suitability.

**Min Players / Max Players are not suitability scores.**

They describe the supported player-count range, but do not indicate how well the game works at each player count.

**Best With / Recommended With are summaries.**

The player-count vote data provides more detail about the underlying distribution of recommendations and should remain available independently of the summary fields.

### Explicitly out of scope

Do not import or model BGG data merely because it exists.

The initial implementation does not require:

- designers
- artists
- publishers
- mechanics
- categories
- expansions
- forums
- comments
- marketplace data
- videos
- images
- user collections
- historical rating data

Additional BGG data may be introduced later only when a concrete application requirement justifies it.

### Data ownership

BGG remains the external source of imported game information.

The BoardGame Library API owns its local representation of that information.

Imported BGG data should therefore be treated as external-source data rather than as an uncontrolled mirror of BGG.

### Contract questions

Before implementation, confirm:

- Which BGG fields are actually available from the selected API response?
- Which fields are optional or can be missing?
- What exact representation does BGG use for player-count categories?
- How should missing BGG values be represented in the local database?
- Which BGG values should be refreshed when an existing game is imported again?