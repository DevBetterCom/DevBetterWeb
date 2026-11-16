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
  the UI later falls back to the member avatar). Photo URL must be an absolute **https** URL. That rules
  out `javascript:`/`data:` schemes and mixed content.
- Admin feedback is cleared on resubmit, so a fresh review starts clean.
- Role checks (who is admin) live in the UI/application layer (#1490/#1491), not in the entity. The
  entity enforces state transitions only.
- Out of scope: use-case handlers for authoring/review (#1490/#1491), UI, home page (#1492). A minimal
  public read contract (`ITestimonialQueries.ListAcceptedAsync`) is included so the module has a real
  public surface and #1492 has something to consume.
- The module has its own `TestimonialsDbContext`, a `testimonials` schema, and its own
  `__EFMigrationsHistory` table in that schema. It shares the physical database (DefaultConnection).

## Phase 1 – Projects and wiring

- [x] Create `src/DevBetterWeb.Modules.Testimonials.Contracts` (no package deps): `TestimonialStatus`, `TestimonialDto`, `ITestimonialQueries`.
- [x] Create `src/DevBetterWeb.Modules.Testimonials` (refs Contracts, EF Core SqlServer, GuardClauses). Types are internal by default, with `InternalsVisibleTo` for the test project.
- [x] Public entry points: `AddTestimonialsModule(IServiceCollection, string? connectionString)` and `ApplyTestimonialsMigrationsAsync(IServiceProvider)`.
- [x] Add both projects to `DevBetterWeb.slnx`, reference them from Web, call `AddTestimonialsModule` in `Program.cs` (SQL Server outside Testing), and apply its migrations with the identity/app contexts.
- [x] Register an in-memory `TestimonialsDbContext` in the FunctionalTests `CustomWebApplicationFactory`, which is the Testing-environment host. The `DevBetterWeb.Tests` factory runs as Development, so `Program.cs` already registers SQL Server for the module there and nothing resolves it. It was left unchanged.

## Phase 2 – Domain

- [x] `Testimonial` aggregate with `Create`, `Edit`, `Submit`, `Accept`, `Decline`, `RequestChanges`, `Unpublish`, `CanBeDeletedByAuthor`, and timestamps (created/updated/submitted/reviewed).
- [x] `AuthorProfile` value object (owned type) with guard clauses and max lengths.
- [x] `InvalidTestimonialStateException` for illegal transitions.

## Phase 3 – Persistence

- [x] `TestimonialsDbContext` (schema `testimonials`) with an entity configuration (owned `AuthorProfile`, enum as string, max lengths, index on `(Status)` and `(MemberId)`).
- [x] Design-time factory and initial migration `AddTestimonials`.
- [x] `TestimonialQueries` implementing `ITestimonialQueries` (accepted only, newest review first, no tracking).

## Phase 4 – Automated tests (`tests/DevBetterWeb.Modules.Testimonials.Tests`, xUnit + Shouldly)

- [x] Unit: every valid transition and every invalid transition (theory over all statuses) for each operation.
- [x] Unit: edit allowed only in Draft/ChangesRequested; request-changes requires feedback; resubmit clears feedback; delete allowed only in Draft.
- [x] Unit: `AuthorProfile` validation (required name, max lengths, photo URL scheme).
- [x] Persistence: the model uses the `testimonials` schema, and the migrations match the model (`HasPendingModelChanges` is false).
- [x] Persistence: in-memory round trip of an owned `AuthorProfile`. `ListAcceptedAsync` returns only accepted items, in order.
- [x] DI: `AddTestimonialsModule` resolves `ITestimonialQueries`.
- [x] Functional: the Testing host resolves `ITestimonialQueries` against its in-memory DB (`tests/DevBetterWeb.FunctionalTests/Modules/TestimonialsModuleRegistration.cs`), and the existing functional tests still pass.

## Phase 5 – Independent review

- [x] A separate agent reviews the full diff against #1489 (correctness, missed requirements, test gaps, security, performance, accessibility).
- [x] Address significant findings, or record the reasons here.
  - **Result:** no blockers or significant findings. The reviewer applied the migration to a throwaway LocalDB, rolled it back, and checked the idempotent script.
  - **Applied:**
    - `EnsureCanBeDeletedByAuthor()` guard, with tests across all statuses.
    - `RowVersion` concurrency token, so concurrent admin reviews fail instead of last-write-wins. The migration was regenerated.
    - Photo URL is now https only.
    - The generated migration class is now `internal`.
    - Added tests: functional-host DI resolution, Declined-via-unpublish is terminal, over-long feedback for Decline/Unpublish, null author, and failed transitions leaving feedback and timestamps unchanged.
    - Documented that `ApplyTestimonialsMigrationsAsync` needs a scoped provider.
  - **Left alone, with reasons:**
    - No default cap on `ListAcceptedAsync(null)`. #1492 picks the limit for its pages.
    - The `Decline(now, feedback?)` parameter order stays, because the optional parameter must come last.
    - The unpublish reason shares `ReviewerFeedback`. #1490 decides whether members see it.
    - Migration `Down` keeps the schema, which is EF's default.
    - No logging in the migration helper. The host logs startup.
  - **Deploy note (for the PR):** production migrations are manual. Run `dotnet ef migrations script --idempotent --context TestimonialsDbContext` (see `efscripts.txt`). The DB login needs CREATE SCHEMA permission.

## Phase 6 – Verification

- [x] Run `dotnet build && dotnet test && dotnet format --verify-no-changes`. Known: repo-wide format failures already exist on `main`, so check that the new or touched files add none.
