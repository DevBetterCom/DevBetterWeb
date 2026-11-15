# Issue #1489 – Testimonials module: domain model and persistence

Issue: https://github.com/DevBetterCom/DevBetterWeb/issues/1489
Parent: https://github.com/DevBetterCom/DevBetterWeb/issues/1472 (split into #1489–#1492, approved by maintainer 2026-11-15)
Branch: `issue-1489-testimonials-module-domain`

## Assumptions / scope

- This is the first real module from `2026-07-29-modular-monolith-recommendations.md` §4.4: a
  `Contracts` project plus an implementation project. No `DevBetterWeb.SharedKernel` project exists yet,
  so the module does **not** reference `DevBetterWeb.Core`. It is self-contained and keeps its own small
  base types, so it does not pull in Core's Vimeo/Identity/AppInsights dependencies.
- The author is referenced by `MemberId` (int) only. There is no `Member` navigation and no FK across modules.
- Status rules (from the approved split):
  - `Draft` → `PendingReview` (submit)
  - `ChangesRequested` → `PendingReview` (resubmit)
  - `PendingReview` → `Accepted` | `Declined` | `ChangesRequested` (request-changes needs feedback)
  - `Accepted` → `Declined` (admin unpublish)
  - `Declined` is terminal
  - Editing is allowed only in `Draft` / `ChangesRequested`. Authors can delete only a `Draft`.
- Display info: display name (required), title, organization, optional photo URL override (null means
  the UI later falls back to the member avatar). Photo URL must be absolute http/https, so dangerous
  schemes like `javascript:` are rejected before they ever reach a page.
- Admin feedback is cleared on resubmit, so a fresh review starts clean.
- Role checks (who is admin) live in the UI/application layer (#1490/#1491), not in the entity. The
  entity enforces state transitions only.
- Out of scope: use-case handlers for authoring/review (#1490/#1491), UI, home page (#1492). A minimal
  public read contract (`ITestimonialQueries.ListAcceptedAsync`) is included so the module has a real
  public surface and #1492 has something to consume.
- The module has its own `TestimonialsDbContext`, a `testimonials` schema, and its own
  `__EFMigrationsHistory` table in that schema. It shares the physical database (DefaultConnection).

## Phase 1 – Projects and wiring

- [ ] Create `src/DevBetterWeb.Modules.Testimonials.Contracts` (no package deps): `TestimonialStatus`, `TestimonialDto`, `ITestimonialQueries`.
- [ ] Create `src/DevBetterWeb.Modules.Testimonials` (refs Contracts, EF Core SqlServer, GuardClauses). Types are internal by default, with `InternalsVisibleTo` for the test project.
- [ ] Public entry points: `AddTestimonialsModule(IServiceCollection, string? connectionString)` and `ApplyTestimonialsMigrationsAsync(IServiceProvider)`.
- [ ] Add both projects to `DevBetterWeb.slnx`, reference them from Web, call `AddTestimonialsModule` in `Program.cs` (SQL Server outside Testing), and apply its migrations with the identity/app contexts.
- [ ] Register an in-memory `TestimonialsDbContext` in both `CustomWebApplicationFactory`s so the Testing host resolves.

## Phase 2 – Domain

- [ ] `Testimonial` aggregate with `Create`, `Edit`, `Submit`, `Accept`, `Decline`, `RequestChanges`, `Unpublish`, `CanBeDeletedByAuthor`, and timestamps (created/updated/submitted/reviewed).
- [ ] `AuthorProfile` value object (owned type) with guard clauses and max lengths.
- [ ] `InvalidTestimonialStateException` for illegal transitions.

## Phase 3 – Persistence

- [ ] `TestimonialsDbContext` (schema `testimonials`) with an entity configuration (owned `AuthorProfile`, enum as string, max lengths, index on `(Status)` and `(MemberId)`).
- [ ] Design-time factory and initial migration `AddTestimonials`.
- [ ] `TestimonialQueries` implementing `ITestimonialQueries` (accepted only, newest review first, no tracking).

## Phase 4 – Automated tests (`tests/DevBetterWeb.Modules.Testimonials.Tests`, xUnit + Shouldly)

- [ ] Unit: every valid transition and every invalid transition (theory over all statuses) for each operation.
- [ ] Unit: edit allowed only in Draft/ChangesRequested; request-changes requires feedback; resubmit clears feedback; delete allowed only in Draft.
- [ ] Unit: `AuthorProfile` validation (required name, max lengths, photo URL scheme).
- [ ] Persistence: the model uses the `testimonials` schema, and the migrations match the model (`HasPendingModelChanges` is false).
- [ ] Persistence: in-memory round trip of an owned `AuthorProfile`. `ListAcceptedAsync` returns only accepted items, in order.
- [ ] DI: `AddTestimonialsModule` resolves `ITestimonialQueries`.
- [ ] Existing functional tests still start the host.

## Phase 5 – Independent review

- [ ] A separate agent reviews the full diff against #1489 (correctness, missed requirements, test gaps, security, performance, accessibility).
- [ ] Address significant findings, or record the reasons here.

## Phase 6 – Verification

- [ ] Run `dotnet build && dotnet test && dotnet format --verify-no-changes`. Known: repo-wide format failures already exist on `main`, so check that the new or touched files add none.
