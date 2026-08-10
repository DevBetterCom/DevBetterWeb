---
name: refactoring-toward-modular-architecture
description: Use when asked to restructure a layered monolith (Core/Infrastructure/Web style) into a modular monolith, extract modules or bounded contexts, or plan a feature-based reorganization — symptoms include a god aggregate with navigation collections into many features, open-generic IRepository<T> letting any code mutate any entity, DI registration scattered across many files, and third-party SDK types (Stripe, Vimeo, etc.) leaking across layers.
---

# Refactoring Toward Modular Architecture

## Overview

Evidence first, boundaries second, sequencing third. The deliverable is a dated recommendations **document** (see [document-template.md](document-template.md)), not code changes. Every boundary claim must cite a file; every phase must leave the app releasable.

## Process

### 1. Inventory the current state (before proposing anything)

Collect, with file paths as evidence:

- **Project + package references**: which SDKs leak where (e.g. Vimeo in Core, MediatR in Core)?
- **Entity graph**: navigation collections on the largest entity — each collection pointing at another feature is a paved-over module boundary. Count lines; find business logic living on it.
- **Repository surface**: open-generic `IRepository<T>` registrations = no enforcement point exists.
- **DbContexts + migration histories** (an existing second context, e.g. Identity, proves the split pattern works).
- **DI registration surfaces**: count them; grep for duplicate registrations at divergent lifetimes — those are latent bugs.
- **Cross-layer usings**: count `using *.Infrastructure` in the web layer.
- **Events + handlers**: who publishes, who subscribes. A pile of `NotifyOnX`/`DiscordLogX` handlers in Infrastructure means the transport owns the policy — inverted.
- **Specs/queries with cross-aggregate Includes**: these break first under any split; list them.
- **Duplicated pages/services, dead code, missing `[Authorize]`**.

### 2. Fix urgent findings immediately

Analysis surfaces real bugs (missing auth attributes, broken email sends). Fix them **now**, before restructuring; mark them ~~struck through~~ "Fixed <date>" in the document. Never let a security gap ride a months-long refactor.

### 3. Draft candidate modules

Cluster by event vocabulary + nav collections + change coupling. For each: **Owns / Interface to other modules / Why it's a context**. Rate cohesion and extraction difficulty in a table. Group small look-alike contexts; keep SharedKernel deliberately tiny (no entities, no business rules); add a supporting Notifications/Ops module (transports only, no policy).

### 4. Name the keystone decision

Usually: who owns the god aggregate and how others reference it. Until cross-module references are **ID-only** (records keyed by `MemberId`, ownership flipped to the referencing module), no other boundary can be enforced. Say this explicitly; sequence around it.

### 5. Map cross-module interactions — then check for cycles

Classify each interaction: **sync query** (small public interface returning DTOs/read models, e.g. `IMembershipQueries.GetSummary(id)`) or **integration event**. Then **list bidirectional module pairs** (A queries B; B displays A's data). Any such pair dictates the project structure below — do not skip this check.

### 6. Target structure: Contracts + implementation project per module

One project per module does NOT work: bidirectional pairs become MSBuild reference cycles, and "reference their Contracts *namespace* only" is unenforceable by the compiler. Per module:

- `Modules.X.Contracts` — public interfaces, DTOs/read models, **the integration events X publishes**. References SharedKernel only. No EF/SDK/handler code.
- `Modules.X` — domain + application + EF + UI, `internal` by default. Public surface is exactly `AddXModule(IServiceCollection, IConfiguration)` + `MapXModule(IEndpointRouteBuilder)`. References other modules' **Contracts projects only**, never implementations.
- Host references implementations solely to call Add/Map. Compiler enforces the seams; arch tests (NetArchTest) cover only what it can't: SDK containment, Contracts purity, no stray public types.

### 7. Phase the roadmap

Order: **hygiene → composition-root discipline (Add*Module regrouping in place) → smallest/cleanest module first to prove the template → ... → god aggregate last → strict enforcement**. Every phase releasable; cheap wins build the pattern before the hard split.

### 8. Record open questions

Policies spanning modules (e.g. graduation = policy over Billing tenure, mutating Identity roles, sending Membership comms) get an "Open question" entry with a recommendation — not a silent decision. The team confirms before the affected phase.

## Gotchas (each one bit a real project)

| Gotcha | Reality |
|---|---|
| "Namespace-based Contracts rules are enough; single project per module" | Arch tests don't stop MSBuild cycles. Find the bidirectional pair first — it forces Contracts projects. |
| Integration event types "somewhere shared" | They live in the **publisher's** Contracts project, or subscribers must reference implementations. |
| "Put the shared DTO in SharedKernel for now" | That's how the kernel becomes a junk drawer and the old architecture returns with more projects. |
| Rewriting EF migration history | One-way door. Freeze old history; each module context starts with a **no-op baseline** against existing tables; rename schemas opportunistically. |
| Cross-aggregate specs "will still work" | `X→comments→authors→progress` eager-loads break first. Replace with module-local queries + summary read models; accept extra round-trips until measured. |
| Admin/reporting pages that join everything | Give them a named, read-only exception (views/read model), or they re-couple modules via contract call chains. |
| Duplicate DI lifetimes "cleaned up silently" | Behavior changes there are latent bugs found, not regressions caused. Fix as isolated, bisectable commits. |
| A giant `Constants.cs` | Dismantle it per owning module; it's hidden coupling. |
| "Since we have modules, microservices next" | Modules make that possible later; doing it now converts compile-time errors into network errors. |

## Red Flags — stop and redo the step

- Proposing a target structure before citing file-level evidence
- A findings list with no file paths
- No bidirectional-pair check before choosing project layout
- A phase that leaves the app unreleasable
- A security finding scheduled for "Phase 5"
