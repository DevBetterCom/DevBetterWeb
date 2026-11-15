# Issue #1486 – Remove duplicate Vimeo DI registration

Issue: https://github.com/DevBetterCom/DevBetterWeb/issues/1486
Branch: `issue-1486-vimeo-di-cleanup`

## Assumptions / scope

- NimblePros.Vimeo **2.3.0** (the target named in the issue comment, which replaces 2.2.1) is
  **not on nuget.org yet**. As of 2026-11-15 the latest version is 2.1.2. So the package upgrade is
  deferred, and the PR will say `Relates to #1486` rather than `Fixes`.
- This PR does the registration cleanup, which the comment says must happen before (or
  together with) the 2.3.0 upgrade. Otherwise startup throws `InvalidOperationException`.
- Manual smoke testing of the admin video flows needs a real Vimeo token and is left to the
  maintainer.

## Phase 1 – Registration cleanup

- [x] Delete `src/DevBetterWeb.Infrastructure/VimeoServiceCollectionExtensions.cs`, including `AuthHeaderHandler`.
- [x] Remove `RegisterVimeoServicesDependencies` call and the `vimeoToken` parameter from `AddInfrastructureServices`.
- [x] Remove the `vimeoToken` local and the commented-out Autofac line from `Program.cs`. Keep `AddVimeoServices(vimeoSettings.Token)`.

## Phase 2 – Automated tests (DevBetterWeb.UnitTests)

- [x] `AddInfrastructureServices` registers no Vimeo services or named clients (guards against the 2.3.0 duplicate-detection startup failure).
- [x] `AddInfrastructureServices` followed by `AddVimeoServices` gives exactly one `ClientApiConfigurations` and one registration of each tus service, and the Vimeo services resolve.

## Phase 3 – Independent review

- [x] Separate agent reviews the full diff against the issue.
- [x] Address significant findings (or record reasons here).

## Phase 4 – Verification

- [x] `dotnet BuildTestFormat.cs` runs clean.

## Deferred

- [ ] Upgrade NimblePros.Vimeo to 2.3.0 once it is published.
- [ ] Smoke test the admin video flows (upload chunk, update description, delete, details, oEmbed).
