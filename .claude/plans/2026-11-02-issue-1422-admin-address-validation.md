# Issue 1422: Admin User page address validation

https://github.com/DevBetterCom/DevBetterWeb/issues/1422

Status at start: #1469 (merged) fixed the blank `userId`, inline errors, and members with no address at all, but did not close the issue. Draft #1423 conflicts with main and is superseded: it removes `[Required]` from the model shared with the member profile page, whose handler would then build an `Address` from null values.

Remaining gaps against the issue's expected behavior:

- Address validation fires for any non-blank address, even if the admin did not touch it. A member with an incomplete stored address or a legacy `Member.Address` value cannot have only their name or email changed. The issue's payload (only `Address` missing) is this case.
- The page does not load the validation scripts, so no client-side validation fires.

Assumption: "updated" means differs from the stored value, ignoring surrounding whitespace and line-ending style. Blanking every address field skips validation and leaves an existing `ShippingAddress` and `Member.Address` untouched; with no `ShippingAddress` record, it clears the legacy `Member.Address`.

## Phase 1: Server

- [x] Load the member before validating in `OnPostUpdatePersonalInfoAsync`.
- [x] Skip address validation and updates when the address is unchanged.
- [x] Leave `UserPersonalUpdateModel` and the member profile page unchanged.

## Phase 2: Client

- [x] Load the validation scripts on the Admin User page.
- [x] Make the address `required` rules conditional, matching the server rule (`wwwroot/js/admin-user.js`).
- [x] Add `Subscription.StartDate` validation messages, since those forms now also validate on the client.

## Phase 3: Tests

- [x] `tests/DevBetterWeb.Tests/Pages/AdminUserModelTests/OnPostUpdatePersonalInfoAsync.cs`: unchanged incomplete address, legacy address, CRLF, edit to incomplete address, complete new address.
- [x] `tests/DevBetterWeb.Tests/Pages/UserPersonalUpdateModelTests/AddressFieldLengths.cs`: lengths enforced before the database rejects them.
- [x] Run the client rule in jsdom against the real jQuery Validation libraries, not committed (the repo has no JS test project, and Chrome cannot start in this environment for lack of system libraries).

## Phase 4: Review

- [x] Independent review by a separate agent; address significant findings.
  - Fixed: blank State stored as null (reads back as a null `ShippingAddress`); blanking all fields half-cleared the address; over-length address values reached the database; whitespace-only values passed the client rule.
  - Not addressed: an invalid stored Discord username also blocks a save, but it shows inline and is not address validation. The member profile page has the same null State bug; it is outside this issue, so it is reported instead of fixed here.
  - Accepted: after a server redisplay, the client compares against the submitted values rather than the stored ones. The server still decides, so nothing invalid is saved.
- [ ] `/codex:review` required by CLAUDE.md: not run, because neither the plugin nor the `codex` CLI is installed in this environment.

## Phase 5: Verification

- [x] Build and all tests pass (`NuGetAudit=false`; main fails restore locally on NU1902 for the pinned ImageSharp).
- [ ] `dotnet BuildTestFormat.cs` does not run clean: build and tests pass, but the format step fails across the repo, including untouched files. On the changed files it reports only line-ending, indentation and encoding differences (`.editorconfig` asks for CRLF, BOM and other indentation; these files are LF in git). They were left in the existing style to keep the diff reviewable.
