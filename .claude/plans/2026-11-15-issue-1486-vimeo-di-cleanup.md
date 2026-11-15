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
  - No blockers. Applied: named-client check now uses `IOptionsMonitor<HttpClientFactoryOptions>`, which catches any configuration style, and the brittle client-name check against package internals was dropped.
  - Left alone: the `Autofac` entry in `DevBetterWeb.ndproj` (still a transitive dependency of NimblePros.Vimeo) and the unrelated commented-out `DomainEventDispatcher` line.
  - Wire change: `Accept` and `Tus-Resumable` are no longer sent twice. The Tus client no longer sets a base address or `Accept`, which is harmless because chunk PATCHes use an absolute upload link. The smoke test should still cover chunked upload.

## Phase 4 – Verification

- [x] `dotnet BuildTestFormat.cs`: build OK, all tests pass (FunctionalTests, Tests, UnitTests).
- [x] Format step: the failures are pre-existing on `main` (around 11k repo-wide whitespace/imports/charset issues, unchanged by the .NET 11 upgrade). This PR adds no new ones: the new test file is clean and the touched files have fewer issues than on `main`.

## Phase 5 – Package upgrade (2.3.0 published later on 2026-11-15)

- [x] Merge latest `main` (Aspire AppHost #1488). Resolved the `Program.cs` conflict, keeping the Aspire block and dropping the Autofac comment.
- [x] Bump `NimblePros.Vimeo` 2.1.2 → 2.3.0 in `Directory.Packages.props`.
- [x] `dotnet BuildTestFormat.cs`: build OK, 315/315 tests pass (AppHost.Tests 4, FunctionalTests 5, Tests 118, UnitTests 188). The functional tests start the full host, so 2.3.0's startup duplicate-registration check passes. The format failures are the same pre-existing ones as on `main`.
- Risk outside this PR: `AddServiceDefaults` (#1488) adds `AddStandardResilienceHandler` to every HttpClient, Vimeo ones included. Its default timeouts (10s per attempt, 30s total) override the Vimeo clients' 60-minute timeout and could break large chunk uploads.

## Phase 6 – Vimeo resilience timeout fix (requested by the maintainer)

- [x] Added `AddDevBetterVimeoServices` in `src/DevBetterWeb.Web/VimeoServiceCollectionExtensions.cs`. It calls `AddVimeoServices` and then `RemoveAllResilienceHandlers()` on `VimeoApi`, `VimeoTusApi` and `VimeoOEmbed`, using the `ServiceConstants` names, so the clients' 60-minute timeout applies again. It runs after `AddVimeoServices`, as 2.3.0 requires.
  - `RemoveAllResilienceHandlers` is marked experimental (EXTEXP0001), so the warning is suppressed with a `#pragma` on that one line. It is the documented way to opt a client out of `ConfigureHttpClientDefaults`. The other options rely on the internal `"{name}-standard"` pipeline naming or mean changing the shared ServiceDefaults.
- [x] `Program.cs` calls `AddDevBetterVimeoServices`.
- [x] Tests in `tests/DevBetterWeb.UnitTests/Web/VimeoServiceCollectionExtensionsTests.cs` build the host with `AddServiceDefaults`, as `Program.cs` does, and check:
  - no `ResilienceHandler` in any Vimeo client's handler chain (these fail without the fix);
  - each Vimeo client keeps its 60-minute timeout;
  - other clients keep their resilience handler.
- [x] `dotnet BuildTestFormat.cs`: build OK, 322/322 tests pass. Running `dotnet format --verify-no-changes` on the solution, limited to the new files, exits 0. The full-solution run still reports one IMPORTS issue at (1,1) of the new Web file, which I could not reproduce in isolation. Every other format issue is pre-existing on `main`.

## Deferred

- [ ] Smoke test the admin video flows (upload chunk, update description, delete, details, oEmbed).
