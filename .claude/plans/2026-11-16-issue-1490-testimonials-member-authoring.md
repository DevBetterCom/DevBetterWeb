# Issue #1490 – Testimonials: member authoring UI

Issue: https://github.com/DevBetterCom/DevBetterWeb/issues/1490
Parent: https://github.com/DevBetterCom/DevBetterWeb/issues/1472 (the run started on the parent and pivoted here: #1489 is closed, and #1490 is the first open, unblocked sub-issue)
Branch: `issue-1490-testimonials-member-authoring`

## Assumptions / scope

- Pages live at `/User/MyProfile/Testimonials` (list) and `/User/MyProfile/Testimonials/Edit[/{id}]`
  (create or edit). They use the same roles as the other MyProfile pages: `ADMINISTRATORS_MEMBERS_ALUMNI`.
- The module exposes a public use-case surface, `ITestimonialAuthoring`, in Contracts. It is keyed by
  `memberId`, and every call filters by `memberId`, so members only see or change their own testimonials.
  Someone else's id behaves exactly like a missing id (NotFound → 404). Calls return a small
  `TestimonialOperationResult` (Success / NotFound / NotAllowed / Invalid with field errors). Ardalis.Result
  was considered, but the Contracts project is documented as dependency-free.
- The Web layer resolves the current `Member.Id` (and a default display name from `UserFullName()`)
  through Core. The module never touches Membership.
- Read-only means the edit page shows the content without a form when `CanBeEditedByAuthor` is false
  (`PendingReview`, `Accepted`, `Declined`). A POST in those states is rejected by the domain
  (`InvalidTestimonialStateException` → `NotAllowed`). The page shows the error and nothing changes.
- Admin feedback (`ReviewerFeedback`) is shown to the author for `ChangesRequested` and `Declined`. This
  includes an unpublish reason, which settles the open question from #1489.
- Submitting is a separate action, available on the list and on the edit page for editable items.
  Deleting is offered only for drafts.
- The photo URL is optional. Help text says a blank value falls back to the member avatar (#1492 renders it).
- The clock comes from `TimeProvider` (`TryAddSingleton(TimeProvider.System)` in the module).

## Phase 1 – Module application surface

- [x] Contracts: `ITestimonialAuthoring`, `TestimonialDraftInput`, `MemberTestimonialDto` (adds status, feedback, timestamps, and `CanEdit`/`CanSubmit`/`CanDelete` flags).
- [x] Module: `TestimonialAuthoring` implementation. It validates input into field errors and maps domain state exceptions/concurrency to `NotAllowed`, ownership misses to `NotFound`. Register it in `AddTestimonialsModule`, along with `TimeProvider`.

## Phase 2 – Razor pages

- [x] `Pages/User/MyProfile/Testimonials/Index` lists the member's testimonials with status and feedback, plus Submit/Delete handlers.
- [x] `Pages/User/MyProfile/Testimonials/Edit` creates or edits, with a read-only view when not editable, a Save handler, and a Save & Submit handler.
- [x] Add a "Testimonials" card/link on `MyProfile/Index`.

## Phase 3 – Automated tests

- [x] Unit (`DevBetterWeb.Modules.Testimonials.Tests/Application`, in-memory DB): create, edit, submit, delete; ownership (another member → NotFound); edit/submit/delete blocked after submit (NotAllowed); invalid input (Invalid); list is scoped to the member.
- [x] Functional (`DevBetterWeb.FunctionalTests/RazorPages/Testimonials`) with a test auth handler that signs in as a seeded member: create, edit, submit, edit-blocked-after-submit, cannot view another member's testimonial (404), anonymous is challenged.

## Phase 4 – Independent review

- [x] A separate agent reviews the full diff against #1490.
- [x] Address significant findings, or record the reasons here.
  - **Result:** no blockers or significant findings.
  - **Applied:**
    - The "Back to My Profile" link resolved to `/User/` and now uses `asp-page`.
    - Added functional tests: feedback shown and resubmitted after ChangesRequested, Accepted is read-only (409 on POST), and the blocked submit/delete error message is displayed. The functional tests get `InternalsVisibleTo` on the module so they can apply admin transitions until #1491 exists.
    - Each card now has a heading, and action buttons use `aria-describedby`, so screen readers can tell the cards apart.
    - The author line is joined with ", " so there is no stray space before the commas (shared `TestimonialDisplay.AuthorLine`).
  - **Left alone, with reasons:**
    - Client `[StringLength]` counts untrimmed text and LF vs CRLF line breaks differently from the server. Near the 4000 limit, the server's message is shown instead. It's harmless.
    - Each save loads the testimonial twice (page status check, then module). It's cheap. Last write wins between two tabs editing the same draft, and admins can't act on Draft/ChangesRequested, so the RowVersion window is acceptable.
  - **Also fixed (found while testing):** `CustomWebApplicationFactory` built a new EF internal service provider per scope for the app and identity DBs, so seed data was invisible to requests. The provider is now built once, matching the testimonials registration.

## Phase 5 – Verification

- [x] `dotnet build && dotnet test && dotnet format --verify-no-changes`. Repo-wide format failures already exist on main, so check that the new or touched files add none.
