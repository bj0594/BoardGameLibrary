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
- solo-discovery behaviour follows the finalized query and data rules;
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

- **Selected:** Not decided.
- **Why:** xUnit is optional. The project already has a verification plan using manual observation, implementation inspection, integration evidence, and documentation evidence. If R23 is selected, the planned automated tests below can be implemented without changing the project contracts.
- **Test project:** [Project name / `Not applicable`]

---

# 2. COVERAGE MAP

`Requirement → behaviour → verification / evidence`

- **R1 →** B01 / B02 / B03 / B04 → INT / M / T if selected
- **R2 →** B02 / B03 → M / T if selected
- **R3 →** B01 → M / INT / T if selected
- **R4 →** B04 → M / T if selected
- **R5 →** B04 / B05 → M / INT / T if selected
- **R6 →** B01 → M / T if selected
- **R7 →** B01 / B02 / B03 / B05 → I / INT / T if selected
- **R8 →** B01 / B02 / B05 → I / INT / T if selected
- **R9 →** B01 / B02 / B05 → I / T if selected
- **R10 →** B01 / B02 / B03 / B04 / B05 → M / INT / T if selected
- **R11 →** B04 / B05 → M / INT / T if selected
- **R12 →** Delivery → DOC
- **R13 →** B01 / B02 / B03 / B04 → I / M / INT / T if selected
- **R14 →** Delivery → DOC
- **R15 →** Manual endpoint verification → M
- **R16 →** Delivery → DOC
- **R17 →** B01 / B02 → INT / T if selected
- **R18 →** B01 / B02 → I / INT / T if selected
- **R19 →** B01 / B02 → I / INT / T if selected
- **R20 →** Delivery → DOC
- **R21 →** Only if a service-layer decision is made
- **R22 →** B03 if GET extensions are selected → M / T if selected
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

### M03 — Discover solo-suitable games
- **Behaviour / requirement:** B03 / R2 / R22 if selected
- **Purpose:** Demonstrate the finalized solo/player-count query behaviour.
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
- **Status:** Planned

### DOC02 — Repository and delivery
- **Requirement:** R12 / R13 / R16
- **Purpose:** Verify required repository contents and GitHub/Canvas delivery.
- **Status:** Planned

---

# 4. AUTOMATED TEST DESIGN

The following tests are the planned automated set if R23 is selected. They can be implemented as API/integration tests rather than tests of internal implementation details.

## T01 — Add valid game and persist local resource

- **Behaviour:** B01
- **Requirements:** R1, R3, R6, R17, R18, R19
- **Scenario:** A valid game identifier and valid local input are submitted and the required external data is available.
- **Data:** Representative valid game and library data.
- **Observation:** HTTP response and database state.
- **Oracle:** Defined creation response is returned and the expected mapped resource is persisted.
- **Level:** API / Integration
- **Dependency strategy:** Controlled external + test database boundary.
- **Proves:** The main create/enrich/persist flow.

## T02 — Retrieve stored games

- **Behaviour:** B02
- **Requirements:** R1, R2, R17, R18, R19
- **Scenario:** Known resources exist in the test database and a valid GET request is submitted.
- **Data:** Representative persisted resources.
- **Observation:** HTTP response and returned resource/collection.
- **Oracle:** Returned data matches the defined API contract and stored state.
- **Level:** API / Integration
- **Dependency strategy:** Controlled test database.
- **Proves:** Correct retrieval from local persistence.

## T03 — Discover solo-suitable games

- **Behaviour:** B03
- **Requirements:** R2, R22 if selected
- **Scenario:** Stored games contain different finalized solo/player-count values and a valid discovery query is submitted.
- **Data:** Matching, non-matching, and relevant boundary cases.
- **Observation:** HTTP response and returned collection.
- **Oracle:** Returned resources satisfy the finalized discovery rule; excluded resources do not.
- **Level:** API / Integration
- **Dependency strategy:** Controlled test database.
- **Proves:** The actual solo-discovery query behaviour.
- **Limitation:** Does not establish that the underlying BGG signal objectively measures solo quality.

## T04 — Reject invalid POST input

- **Behaviour:** B04
- **Requirements:** R4, R5, R10, R11
- **Scenario:** POST input violates a finalized validation rule.
- **Data:** Missing, invalid, or boundary-invalid request data as applicable.
- **Observation:** HTTP response and database state.
- **Oracle:** Defined validation response is returned and no invalid resource is persisted.
- **Level:** API / Integration
- **Dependency strategy:** Controlled test database and external boundary.
- **Proves:** Invalid requests cannot create invalid local state.

## T05 — Handle external dependency failure

- **Behaviour:** B01 / B05
- **Requirements:** R5, R8, R11
- **Scenario:** The BGG boundary is unavailable, times out, or returns an unusable response in a controlled test setup.
- **Data:** Otherwise valid request data.
- **Observation:** HTTP response and resulting state.
- **Oracle:** Defined failure response is returned and invalid/incomplete state is not persisted.
- **Level:** Integration
- **Dependency strategy:** Stub / controlled external boundary.
- **Proves:** Relevant external-failure handling.

## T06 — Handle persistence failure

- **Behaviour:** B01 / B02 / B05
- **Requirements:** R8, R11, R19
- **Scenario:** The database boundary fails during create or read.
- **Data:** Valid representative data.
- **Observation:** HTTP response and resulting state.
- **Oracle:** Defined failure response is returned and the API does not report an unsuccessful persistence operation as successful.
- **Level:** Integration
- **Dependency strategy:** Controlled database failure.
- **Proves:** Relevant database-failure handling.

## T07 — Duplicate game handling

- **Behaviour:** B01 / B04
- **Requirements:** R4, R5, R11
- **Scenario:** The same game is submitted again according to the finalized duplicate rule.
- **Data:** Repeated valid identifier and relevant library data.
- **Observation:** HTTP response and database state.
- **Oracle:** Response and persisted state follow the finalized duplicate rule.
- **Level:** API / Integration
- **Dependency strategy:** Controlled test database and external boundary.
- **Proves:** Duplicate behaviour is explicit and consistent.

> If automated testing is not selected, T01–T07 remain verification designs rather than implementation commitments.

## Relevant test dimensions

Consider only categories that reveal meaningful failures:

- [ ] Happy path
- [ ] Boundary values
- [ ] Equivalence partitions
- [ ] Null / empty
- [ ] Invalid input
- [ ] Missing / not found
- [ ] Duplicate
- [ ] Dependency failure
- [ ] Data/type variation

Do not create a separate test for every checked category. Reuse existing tests when they already provide the needed evidence.

---

# 5. TEST DESIGN REVIEW

Before an automated test is considered ready:

- [ ] Referenced behaviour exists in `Planning.md`.
- [ ] Requirement(s) are known.
- [ ] Scenario and input are concrete.
- [ ] Observation point is clear.
- [ ] Oracle is clear.
- [ ] Test level matches the boundary being proven.
- [ ] Dependency strategy is deliberate.
- [ ] Relevant edge cases have been considered.
- [ ] HTTP, validation, persistence, and external-failure behaviour are covered where relevant.
- [ ] A subtly incorrect implementation would be caught.
- [ ] The test does not invent a requirement or behaviour.

If a test is difficult to specify, revisit the behaviour contract, observation boundary, responsibility, dependency boundary, or oracle before adding complexity.

---

# 6. NON-AUTOMATED EVIDENCE

Record only evidence that automated tests do not adequately establish.

## I01 — Async implementation inspection

- **Requirement / behaviour:** R7 / R8 / R9 / R19 / B01 / B02 / B05
- **Property:** External HTTP and database I/O use asynchronous, non-blocking operations without synchronous blocking calls in async methods.
- **Why:** Internal implementation strategy cannot be fully established from API output.

## I02 — Controller responsibility inspection

- **Requirement / behaviour:** R13 / B01 / B02 / B03 / B04
- **Property:** Controllers remain responsible for the HTTP boundary rather than absorbing unnecessary domain or persistence logic.
- **Why:** Responsibility structure is an implementation property.

## M01–M04 — Manual API verification

- **Requirements / behaviours:** R2, R3, R4, R5, R6, R10, R15 / B01–B04
- **Property:** Required API behaviour can be exercised and demonstrated through an API client.
- **Why:** Manual endpoint verification is explicitly required by the assignment.

## DOC01–DOC02 — Documentation and delivery

- **Requirements:** R12, R13, R14, R16, R20
- **Property:** Repository structure, README, database setup documentation, and GitHub/Canvas delivery are complete.
- **Why:** These are delivery requirements rather than runtime behaviour.

---

# 7. IMPLEMENTATION READINESS

This is the hand-off gate from planning into implementation.

The entire test suite does not need to exist as code yet, but the intended verification must be designed sufficiently to prevent invention during implementation.

## Planning readiness

- [ ] `Planning.md` is complete enough to implement from.
- [x] Project direction and core scope are clear.
- [ ] Remaining BGG data and solo-signal decisions are finalized.
- [ ] Final API routes and response contracts are defined.
- [ ] Final domain fields and validation rules are defined.
- [ ] Relevant external and database failure contracts are defined.
- [x] SQL persistence and EF Core direction are decided.
- [ ] Any needed visualization has been completed.

## Verification readiness

- [ ] Important requirements have evidence coverage.
- [ ] Important behaviours have verification methods.
- [ ] Required manual, integration, and inspection evidence is identified.
- [ ] If R23 is selected, all selected automated tests have sufficient design information.
- [ ] No verification item requires inventing an unrecorded requirement or behaviour.

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

- **Chosen strategy:** [Test-first batch / Incremental TDD / Hybrid]
- **Why:** [Short reason.]

---

## Source-of-truth rule

- `Planning.md` → requirements, direction, scope, behaviours, acceptance contracts, domain rules, API/design decisions, and technical baseline.
- `TestPlan.md` → verification strategy, coverage, automated test design, non-automated evidence, and implementation readiness.
- Concrete test file → executable automated test behaviour.
- Todoist → work status and task completion when Todoist is used.

Do not duplicate behaviour contracts or requirements between the two planning files. Reference the owner instead.
