# Issue #1484 — Add .NET Aspire (AppHost + ServiceDefaults) to main

https://github.com/DevBetterCom/DevBetterWeb/issues/1484

Branch: `issue-1484-add-aspire`

## Assumptions

- Latest Aspire = 13.6.5 (Aspire.AppHost.Sdk / Aspire.Hosting.*); ServiceDefaults packages at
  Microsoft.Extensions.* 11.0.0 and OpenTelemetry 1.21.0. All projects target net11.0.
- docker-compose only models SQL Server, so SQL Server is the only AppHost dependency.
  Azure Storage (Azurite) and email are not in docker-compose and stay out of scope.
- The existing `/health` endpoint must keep working in every environment (used in prod), so
  ServiceDefaults maps `/health` and `/alive` unconditionally instead of Development-only.
- The standard resilience handler must not break Vimeo uploads (60 min client timeouts) or
  duplicate non-idempotent POSTs: retries disabled for unsafe methods, and the Vimeo clients
  opt out of the default resilience pipeline.
- Serilog owns logging; logs are forwarded to OpenTelemetry providers only when an OTLP
  endpoint is configured (i.e. under Aspire), so production logging is unchanged.
- Local migrations currently only run in Development inside a container. The AppHost sets
  `ApplyLocalMigrations=true` so the Development web project migrates the fresh Aspire DB.

## Phase 1 — ServiceDefaults project
- [x] Add `src/DevBetterWeb.ServiceDefaults` (net11, IsAspireSharedProject) with `Extensions.cs`
- [x] Central package versions for ServiceDiscovery, Http.Resilience, OpenTelemetry.*

## Phase 2 — AppHost project
- [x] Add `src/DevBetterWeb.AppHost` (Aspire.AppHost.Sdk 13.6.5, net11)
- [x] Model SQL Server (persistent data volume) + `DevBetterWeb` database, referenced by web
      as `DefaultConnection`; web waits for the database
- [x] Pass `ApplyLocalMigrations=true` to web
- [x] Add both projects to `DevBetterWeb.slnx`

## Phase 3 — Wire Web
- [x] Reference ServiceDefaults; call `AddServiceDefaults()` / `MapDefaultEndpoints()`
- [x] Replace ad-hoc `AddHealthChecks` / `MapHealthChecks("/health")`
- [x] Serilog `writeToProviders` when OTLP endpoint configured
- [x] Honor `ApplyLocalMigrations` config flag
- [x] Vimeo HttpClients remove default resilience handlers

## Phase 4 — Tests
- [x] New `tests/DevBetterWeb.AppHost.Tests` using Aspire.Hosting.Testing: build the app model
      (no containers started) and assert sql/db/web resources, connection-string reference,
      WaitFor, and `ApplyLocalMigrations` env var
- [x] Unit/functional coverage for `/health` and `/alive` endpoints in Testing environment
- [x] Existing MigrationServiceExtensions tests still pass

## Phase 5 — Docs
- [x] README: "Run with .NET Aspire" section (prereqs: Docker/Podman, `aspire run` or
      `dotnet run --project src/DevBetterWeb.AppHost`)

## Phase 6 — Independent review
- [ ] Separate agent reviews full diff vs issue
- [ ] Address significant findings (record skipped ones here)

## Phase 7 — Verification
- [ ] `dotnet BuildTestFormat.cs` clean (build, tests, format)
- [ ] Open PR with `Fixes #1484`
