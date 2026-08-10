# DevBetterWeb — Modular Monolith Restructuring Recommendations

**Date:** 2026-07-29
**Status:** Proposal / recommendations (no code changes yet)
**Scope:** Full solution analysis — `DevBetterWeb.Core`, `DevBetterWeb.Infrastructure`, `DevBetterWeb.Web`, and the three test projects.

---

## 1. Executive Summary

DevBetterWeb is currently a classic three-layer Clean Architecture solution (Core / Infrastructure / Web). The layering is *technical*, not *domain-driven*: every domain concept is smeared horizontally across all three projects, and the seams that exist (Core interfaces, generic `IRepository<T>`) do not stop any feature from reaching into any other feature's data.

The analysis identified **seven candidate bounded contexts** plus a shared kernel:

| # | Module | Cohesion today | Extraction difficulty |
|---|--------|----------------|----------------------|
| 1 | **Identity & Access** | High — already has its own DbContext + migrations | Low (first extraction candidate) |
| 2 | **Membership & Profiles** | Low — `Member` is a god aggregate | High (do last, incrementally) |
| 3 | **Billing & Subscriptions** | Medium — cohesive Stripe code, but leaks Stripe types and mutates Member | Medium-High |
| 4 | **Video Library** | High — largest, cleanest business area | Medium |
| 5 | **Coaching & Q&A** | High — small, self-contained | Low |
| 6 | **Books & Leaderboard** | High — small, clean seam | Low |
| 7 | **Notifications & Ops** (supporting) | Cross-cutting — Discord/email/daily-check | Medium (redistribute, then shrink) |

The single highest-leverage design decision is **who owns `Member` and how other modules reference it**. Today `Member` (~500 lines) owns profile, geolocation, social links, books read/uploaded, video progress, video comments, questions, votes, favorites, subscriptions, and billing activities — it is the universal join entity, injected via `IRepository<Member>` in 20+ page models and services across every feature. Until cross-module references to `Member` become ID-only, no other boundary can be enforced.

---

## 2. Current State — Key Findings

### 2.1 Structure

- **3 projects**: `Core` (entities, events, interfaces, specs, some services), `Infrastructure` (EF, Stripe, Vimeo, Discord, email, Identity, event handlers, many services), `Web` (Razor Pages, Ardalis.ApiEndpoints, MVC controllers, more services).
- **Two DbContexts** on one connection string: `AppDbContext` (13 DbSets + 2 config-only entities, 26 migrations) and `IdentityDbContext` (ASP.NET Identity, 2 migrations).
- **MediatR is used for notifications only** (`Publish`, no `Send`) — domain events fan out to `IHandle<T>` implementations, almost all of which are Discord loggers in `Infrastructure/Handlers/`.

### 2.2 Coupling hotspots (evidence the layering isn't holding)

1. **`Member` god aggregate** (`src/DevBetterWeb.Core/Entities/Member.cs`): 6 navigation collections into 4 other candidate modules; billing logic (`AddSubscription`, `ExtendCurrentSubscription`, `AddBillingActivity`, `TotalSubscribedDays`) lives inside the membership entity; a MediatR event handler (`MemberAddressUpdatedHandler`) is nested *inside the entity class* and does Google-Maps JSON parsing.
2. **Every entity is `IAggregateRoot`** (13 of 13) and `IRepository<T>` is open-generic, so any code can load/mutate any module's data. There is no enforcement point.
3. **`NewMemberService`** (Core) spans Invitations + Stripe + Identity roles + email + member creation + billing activity in one class. **`WebhookHandlerService`** (Infrastructure) has 13 injected dependencies and contains the full subscription-lifecycle orchestration, with hardcoded plan logic.
4. **60 files in Web have `using DevBetterWeb.Infrastructure.*`** — including Stripe webhook endpoints referencing `StripeOptions` directly. Four Infrastructure-local interfaces return raw `Stripe.*` types, so Stripe flows into the UI layer.
5. **Vimeo SDK leaks into Core**: `Core.csproj` references `NimblePros.Vimeo`; `CreateVideoService` / `AddCreatedVideoToFolderService` depend on concrete Vimeo classes, not interfaces. Core also references MediatR, `Microsoft.Extensions.Identity.Core`, and Application Insights.
6. **DI registration is scattered across 6 surfaces** (Program.cs, two Web extension classes, two Infrastructure extension classes, and an `IdentityHostingStartup` assembly attribute) with **duplicate registrations at divergent lifetimes** (Stripe handlers registered Transient in Infrastructure and Scoped in Web; Vimeo registered twice).
7. **Domain logic lives in the wrong layer everywhere**: graduation policy, invitation ping cadence, and webhook orchestration in Infrastructure; leaderboard ranking, book, and video-details services in Web; Vimeo upload orchestration in Core.
8. **Cross-context queries baked into specs**: e.g. `ArchiveVideoByVideoIdFullAggregateSpec` eager-loads video → comments → Member authors → replies → progress in one query. These break first under any split and need read-model replacements.
9. **The "Notifications" concern is inverted**: 20+ `DiscordLog*Handler` classes in Infrastructure subscribe to every module's events — the transport (Discord) owns the policy (what to announce) instead of each module owning its own announcements.
10. **Duplicated logic**: member cancellation implemented in both `Pages/Admin/Members/Cancel` and `Pages/User/MyProfile/Cancel`; two near-identical Map pages; two `BookViewModel` classes; duplicated Vimeo DI.

### 2.3 Hygiene issues to fix regardless of restructuring

- ~~`Pages/Map/Index`, `Pages/User/Map`, and `SubscriptionStatusController` have **no authorization attribute**.~~ **Fixed 2026-07-29** — maps now require `Administrators,Members,Alumni`; subscription-status requires `Administrators`.
- ~~`GraduationCommunicationsService` sends the member's congratulation email to an empty address (`var memberEmail = "";`).~~ **Fixed 2026-07-29** — resolves the email via `UserManager.FindByIdAsync(member.UserId)` with `member.Email` fallback.
- Dead code: `PaymentIntentUpdateController` (fully commented out), `NewMemberCreatedAndProfileUpdatedEvent`, `BookUpdatedEvent` (never registered), stub `MemberVideoService` inside `MemberVideoProgress.cs`.
- Namespace mismatches: several Infrastructure handlers declare `namespace DevBetterWeb.Core.Handlers`; `IdentityDbContext` declares `namespace DevBetterWeb.Web.Models`; `DiscordWebooks` typo.
- `AppDbContextFactory` has a hardcoded localdb connection string.
- `Smtp2GoEmailService` constructs its own `new HttpClient()`.

---

## 3. Identified Bounded Contexts

### 3.1 Identity & Access
Authentication, roles (`Administrators` / `Members` / `Alumni`), email confirmation, password reset, API-key auth for the uploader.

- **Owns:** `ApplicationUser`, `IdentityDbContext` + migrations, Identity Razor area, `Pages/Admin/Users`, `Pages/Admin/Role`, role/lookup/email-confirmation services, `UploaderApiAuthorization*` filters, the string-keyed `User*` / `PasswordReset` / `InvalidUser` events.
- **Interface to other modules:** `IUserLookupService` (email ↔ userId, role checks), `IUserRoleMembershipService` (add/remove role). Keyed by `string userId`.
- **Why it's a context:** different key type (`string` userId vs `int` memberId), own DbContext, own migrations, no entity references in or out. The seam already exists — it just needs the `IdentityHostingStartup` magic dissolved into an explicit module registration.

### 3.2 Membership & Profiles
The member directory: profile, avatar, social links, birthday, shipping address, geolocation/map, alumni status, invitations/onboarding.

- **Owns:** `Member` (slimmed — see §4.1), `Invitation`, registration + `NewMemberService` orchestration, `AlumniGraduationService` + graduation policy, `GoogleMapCoordinateService`, member directory/profile/map pages, `Pages/Admin/Members` and `Pages/Admin/Invitations`.
- **Interface to other modules:** `MemberId` lookup by userId/email; a small `MemberSummary` read model (id, display name, avatar URL) for other modules' display needs; `MemberCreated` / `MemberGraduated` integration events.
- **Note:** Onboarding/invitations could be its own micro-module later; today it is too entangled with billing and identity to stand alone — keep it inside Membership and let `NewMemberService` become the module's application-layer use case.

### 3.3 Billing & Subscriptions
Everything money: Stripe checkout, webhooks, subscription lifecycle, plans, billing activity audit, admin reporting, cancellation.

- **Owns:** `MemberSubscription`, `MemberSubscriptionPlan`, `BillingActivity`, `BillingDetails`/`DateTimeRange` VOs, all `PaymentHandler`/`InvoiceHandler`/`IssuingHandler`/`SubscriptionHandler` Stripe adapters, `WebhookHandlerService` (decomposed — see §4.3), the four Stripe webhook endpoints, `Pages/Checkout`, `MyProfile/Billing` + both Cancel pages (unified), `Admin/SubscriptionPlans`, `Admin/ManageSubscriptions`, `Admin/UserReports`, CSV export.
- **Interface to other modules:** `SubscriptionActivated` / `SubscriptionRenewed` / `SubscriptionEnded` / `MemberCancelled` integration events; query interface for "is member N current / days subscribed / graduation progress".
- **Critical rule:** Stripe types must not cross the module seam. The four Infra-local interfaces that return `Stripe.Invoice` / `Stripe.Issuing.*` / `Stripe.Subscription` get DTO-returning replacements.

### 3.4 Video Library
Recorded coaching sessions: Vimeo-backed catalog, ingest/upload, thumbnails, plus member engagement (watch progress, favorites, comments).

- **Owns:** `ArchiveVideo`, `VideoComment`, `MemberVideoProgress`, `MemberFavoriteArchiveVideo`, `VideoWatchedStatus`, the 16 `VideoEndpoints`, `Pages/Videos`, `Admin/Videos`, `Admin/ArchivedVideos`, `VideosService`, `CreateVideoService`, `AddCreatedVideoToFolderService`, WebVTT parsing, Vimeo HTTP clients.
- **Interface to other modules:** `VideoPublished` integration event; video lookup by id for Coaching's question links.
- **Internal sub-seam** (keep internal for now): *Ingest* (API-key uploader endpoints, Vimeo sync, thumbnails) vs *Engagement* (progress, favorites, comments — keyed by `MemberId`).
- **Fix on the way in:** move the Vimeo SDK dependency out of Core entirely; the module's application layer defines ports, the Vimeo adapter satisfies them.

### 3.5 Coaching & Q&A
Live session scheduling and the member question queue with voting.

- **Owns:** `CoachingSession`, `Question`, `QuestionVote`, the 4 `CoachingSessionEndpoints`, `Pages/CoachingSessions`, the public `Pages/Questions` page.
- **Interface to other modules:** references `MemberId` (question author, voter) and `ArchiveVideoId` (session recording link) as plain IDs — both already FK-only in places, making this one of the easiest extractions.

### 3.6 Books & Leaderboard
Recommended-reading list, categories, who-read-what, and the reading leaderboard.

- **Owns:** `Book`, `BookCategory`, a new `MemberBookRead` join (see §4.1), `Pages/Books`, `Pages/Leaderboard`, `Admin/Books`, `MyProfile/Books` + `BooksAdd`, and the Web-layer `BookService` / `LeaderboardService` / `RankingService` / `RankAndOrderService` family (which is domain logic and moves into the module's application layer).
- **Interface to other modules:** `MemberId`-keyed reads; consumes Membership's `MemberSummary` for leaderboard display.

### 3.7 Notifications & Ops (supporting module, not a business context)
Transports and scheduling only — no business policy.

- **Owns:** email transports (`Smtp2GoEmailService`, `LocalSmtpEmailService`, `DefaultEmailSender`), Discord webhook client + channel wrappers, `BackgroundTaskQueue`/`BackgroundTaskService`, `DailyCheckService` scheduler + `DailyCheck` marker entity, `StartupNotificationService`, ops events (`AppStartedEvent`, `ExceptionEvent`, `SiteErrorOccurredEvent`).
- **What moves out:** the 20+ `DiscordLog*` / `NotifyOn*` handlers redistribute to the module that owns each triggering event; each becomes a thin call to `IEmailSender` / `IDiscordChannel`. The `DailyCheckInitiatedEventHandler` god-orchestrator dissolves — each module subscribes to a published `DailyCheckInitiated` event and runs its own job (Membership: invitation pings + graduation; Billing: plan-count reconciliation; Video: thumbnail sync).

### 3.8 Shared Kernel
Deliberately tiny: `BaseEntity`, domain-event base + dispatcher abstractions, `IRepository<T>` *base definitions* (see §4.2), result/guard helpers, `DateTimeExtensions`, `IMarkdownService`/`IJsonParserService`-style utilities. **No entities, no business rules.** `Constants.cs` is dismantled — role names to Identity, Vimeo domain to Video, Stripe endpoint to Billing, book length to Books.

---

## 4. Key Design Decisions

### 4.1 Break up the `Member` god aggregate (highest leverage)

`Member` sheds every collection that belongs to another module. Ownership flips to the referencing module, keyed by `MemberId`:

| Today on `Member` | Moves to | As |
|---|---|---|
| `MemberSubscriptions`, `BillingActivities`, `AddSubscription`, `ExtendCurrentSubscription`, `AddBillingActivity`, `TotalSubscribedDays` | Billing | `MemberSubscription`/`BillingActivity` aggregates keyed by `MemberId`; tenure calculations in Billing's `SubscriptionPeriodCalculations` |
| `MemberVideoProgress`, `VideosComments`, `MemberFavoriteArchiveVideos` | Video Library | engagement records keyed by `MemberId` |
| `Questions`, `QuestionVotes` | Coaching & Q&A | already FK-shaped |
| `BooksRead`, `UploadedBooks` | Books | explicit `MemberBookRead` join entity (replaces the implicit EF many-to-many) |
| Nested `MemberAddressUpdatedHandler` | deleted | the Infrastructure duplicate (thinned) is the only handler |

What remains is a genuinely deep Membership aggregate: identity link (`UserId`), profile, links, address, geolocation, alumni status. Other modules display member names/avatars via a `MemberSummary` read model exposed by Membership — never by loading the `Member` entity.

### 4.2 Repository and data access become module-internal

The open-generic `IRepository<T>` registration is what lets billing code mutate members and page models query everything. Replace with:

- Each module registers repositories **only for its own aggregates** (can still reuse the Ardalis.Specification `EfRepository<T>` implementation internally).
- Specs move into the owning module; cross-context specs (`ArchiveVideoByVideoIdFullAggregateSpec`, `BookCategoriesSpec` with member joins, `MemberByUserIdWithBooksReadAndMemberSubscriptionsSpec`) are replaced by module-local queries composed in the page/endpoint via each module's public query interface.
- **Database:** keep one physical database; give each module its own schema (`membership.*`, `billing.*`, `video.*`, …) and its own `DbContext` + migration history. Migrate incrementally: table-rename migrations per module as each is extracted. `IdentityDbContext` already proves the pattern works.

### 4.3 Cross-module communication: events first, queries second

- **Commands/workflows** cross seams via **integration events** (the existing MediatR `Publish` infrastructure is already notification-only — keep it, but formalize an `IntegrationEvent` base in the shared kernel and make handlers module-owned). Each integration event *type* lives in the **publishing module's Contracts project** (see §4.4), so subscribers reference contracts, never implementations.
- The two worst orchestrators are decomposed along event lines:
  - `WebhookHandlerService` (Billing) stops calling Identity roles and Member mutation directly. It records the subscription facts and publishes `SubscriptionActivated` / `SubscriptionEnded`; Membership grants/revokes the `Members` role and updates its own state in response; Notifications sends the emails its module owns.
  - `NewMemberService` becomes Membership's onboarding use case, consuming Billing's subscription-verification query and Identity's role service through their public interfaces only.
- **Synchronous queries** across modules go through small public interfaces returning DTOs (`IMembershipQueries.GetSummary(memberId)`, `IBillingQueries.GetSubscriptionStatus(memberId)`), never entities.

### 4.4 Target solution structure

Recommended shape — **two projects per module: a `Contracts` project plus an implementation project** — with shared kernel and thin host (folder-per-module inside fewer projects is an acceptable first step, but project boundaries are what makes the compiler enforce the seams):

```
src/
  DevBetterWeb.SharedKernel/                      (BaseEntity, events base, repo base, utilities)
  DevBetterWeb.Modules.Identity.Contracts/        (public interfaces, DTOs/read models,
  DevBetterWeb.Modules.Identity/                   integration events the module publishes)
  DevBetterWeb.Modules.Membership.Contracts/      (implementation = domain + app + EF
  DevBetterWeb.Modules.Membership/                 + Razor area + endpoints; internal)
  DevBetterWeb.Modules.Billing.Contracts/
  DevBetterWeb.Modules.Billing/
  DevBetterWeb.Modules.Videos.Contracts/
  DevBetterWeb.Modules.Videos/
  DevBetterWeb.Modules.Coaching.Contracts/
  DevBetterWeb.Modules.Coaching/
  DevBetterWeb.Modules.Books.Contracts/
  DevBetterWeb.Modules.Books/
  DevBetterWeb.Modules.Notifications.Contracts/   (IEmailSender, IDiscordChannel,
  DevBetterWeb.Modules.Notifications/              DailyCheckInitiated event)
  DevBetterWeb.Web/                               (host: Program.cs, layout, public/marketing pages,
                                                   composition root calling AddXxxModule() per module)
tests/
  DevBetterWeb.Modules.<Name>.Tests/    (one per module, testing through the module's interface)
  DevBetterWeb.IntegrationTests/        (cross-module flows: onboarding, webhook→role, daily check)
```

**Why Contracts projects and not one project per module: circular dependencies.** This codebase already needs two-way communication between Membership and Billing — Membership consumes Billing's tenure/subscription query (graduation policy, `NewMemberService`) and handles Billing's `SubscriptionActivated`/`SubscriptionEnded` events (whose *types* Billing defines), while Billing's admin pages (`UserReports`, `ManageSubscriptions`) display member names via Membership's `MemberSummary`. With one project per module that is a project-reference cycle MSBuild refuses to build, and the escape hatches are all worse: dumping shared DTOs/events into `SharedKernel` (turns the deliberately tiny kernel into a junk drawer) or forbidding sync queries in one direction (distorts the design to satisfy the build). With a Contracts split the graph is acyclic by construction: implementations depend on Contracts projects, and Contracts projects depend on (at most) `SharedKernel`.

A module's Contracts project holds its public query/command interfaces, DTOs/read models, and the integration events it publishes (e.g. `Billing.Contracts`: `IBillingQueries`, `SubscriptionStatusDto`, `SubscriptionActivated`). It must stay dependency-free apart from `SharedKernel` — no EF, no Stripe, no MediatR handler implementations.

Per-module internal layout (implementation project, folders): `Domain/`, `Application/` (use cases + implementations of the module's own contracts), `Infrastructure/` (EF config, external adapters), `UI/` (Razor pages via `AddRazorPagesOptions` area conventions, ApiEndpoints). Implementation types are **`internal` by default**; the only public surface is `AddXxxModule(IServiceCollection, IConfiguration)` and `MapXxxModule(IEndpointRouteBuilder)` — this replaces today's six scattered DI surfaces and the `IdentityHostingStartup` magic.

**Reference rules (the compiler now enforces the big ones; architecture tests, e.g. NetArchTest, cover the rest):**
- Compiler-enforced: an implementation project references `SharedKernel`, its own Contracts, and other modules' **Contracts projects only** — never another module's implementation. Contracts projects reference `SharedKernel` only. Host references implementation projects solely to call `AddXxxModule`/`MapXxxModule`; no module references the host.
- Arch-test-enforced (what the compiler can't see): `Stripe.*`, `NimblePros.Vimeo.*` types never appear outside their owning module; Contracts projects stay free of infrastructure packages; implementation projects expose no public types beyond the Add/Map entry points.

### 4.5 What stays shared deliberately

- **Razor layout/shell** (`_MemberLayout`, nav) stays in the host; modules contribute nav items via a small `IMenuContributor`-style interface rather than the layout knowing every module's routes.
- **AutoMapper profiles** move into their owning module (registration stays assembly-scan, but per module assembly).
- **Serilog, App Insights, health checks, Swagger** stay host-level.

---

## 5. Phased Migration Roadmap

Each phase leaves the app releasable. Order chosen so cheap wins build the pattern before the hard `Member` split.

### Phase 0 — Hygiene (no structural change)
1. ~~Fix auth gaps (`Pages/Map/Index`, `Pages/User/Map`, `SubscriptionStatusController`) and the empty-email graduation bug.~~ **Done 2026-07-29.**
2. Delete dead code (commented controller, unused events, stub classes, commented DI lines); fix handler/DbContext namespaces and the `DiscordWebooks` typo.
3. Deduplicate DI: single source of truth per service, resolve the Transient-vs-Scoped Stripe conflicts, remove the duplicate Vimeo registration path, dissolve `IdentityHostingStartup` into Program.cs.
4. Consolidate duplicated pages/logic (two Cancel flows, two Map pages, two BookViewModels).

### Phase 1 — Composition-root discipline
1. Introduce `AddXxxModule()` extension per candidate module (still inside existing projects — pure registration regrouping).
2. Split `Constants.cs` per future module. Remove MediatR/Identity/AppInsights/Vimeo package references from Core (move the two Vimeo services out of Core to Infrastructure as a stopgap).
3. Add architecture tests asserting current *intended* rules so regressions surface during the migration.

### Phase 2 — First extractions: Books, then Coaching
Smallest, cleanest contexts; they prove the module template (Contracts + implementation project pair, schema migration, internal-by-default visibility, module tests). Books requires introducing `MemberBookRead` and cutting `Book.MembersWhoHaveRead` ↔ `Member.BooksRead` navigations — the first real ID-only cut, done where blast radius is smallest.

### Phase 3 — Identity module
Mechanical move (own DbContext already); formalize `IIdentityContracts` (lookup, roles, email confirmation). All `UserManager<ApplicationUser>` usage outside the module is replaced by contract calls — this removes the "two databases stitched in the UI" pattern from ~10 page models.

### Phase 4 — Video Library module
Move entities + endpoints + pages; cut `Member.VideosComments`/`MemberVideoProgress`/favorites navigations (engagement records keyed by `MemberId`); replace `ArchiveVideoByVideoIdFullAggregateSpec` with module-local queries + `MemberSummary` lookups; seal Vimeo behind module ports.

### Phase 5 — Billing module
DTO-ify the four Stripe-leaking interfaces; move webhook endpoints + checkout + admin billing pages; decompose `WebhookHandlerService` into per-webhook use cases publishing integration events; move subscription/billing collections off `Member`.

### Phase 6 — Membership module + Notifications shrink
`Member` is now already slim; move what remains, plus invitations/onboarding (`NewMemberService` rebuilt on Billing/Identity contracts). Redistribute the Discord/email handlers to their owning modules; dissolve the daily-check god-handler into per-module subscribers.

### Phase 7 — Enforcement & cleanup
Turn architecture tests strict (fail on any cross-module type reference outside Contracts); per-module test suites replace the cross-cutting unit tests that assert through `Member`; delete the now-empty generic plumbing.

---

## 6. Risks & Open Questions

- **EF migration surgery** (one history → per-module histories/schemas) is the riskiest mechanical step. Mitigation: freeze the existing history, start each module's context with a no-op baseline migration against existing tables, rename schemas opportunistically.
- **Read-model performance**: replacing eager-loaded cross-context specs with per-module queries adds round-trips on pages like Video Details and Leaderboard. Acceptable at devBetter's scale; add module-level caching only if measured.
- **Solo/small-team overhead**: ~16 projects (Contracts + implementation per module, shared kernel, host) is more ceremony than 3. The payoff is compiler-enforced seams, cycle-free cross-module references, and `internal` implementations; Contracts projects are tiny (interfaces/DTOs/events only) so the per-project cost is low. If it still feels heavy, the fallback is folder-per-module in two projects (Modules + Host) with architecture tests doing the enforcement — weaker but cheaper; the Contracts *namespaces* should still exist so the later split stays mechanical.
- **Open question — Alumni/Graduation ownership**: it is a policy over Billing tenure that mutates Identity roles and sends Membership comms. Recommended: Membership owns the policy, consuming Billing's tenure query and publishing `MemberGraduated` (Identity reacts by swapping roles). Worth confirming before Phase 5.
- **Open question — one database schema-split vs. eventual DB-per-module**: recommendation assumes one DB with schemas indefinitely; nothing in the design blocks a later physical split.

---

## 7. Summary of Module → Current-Code Mapping (quick reference)

| Module | Entities | Key services/orchestrators today | Web surface today |
|---|---|---|---|
| Identity | `ApplicationUser` | `UserLookupService`, role services, email-confirmation | `Areas/Identity`, `Admin/Users`, `Admin/Role` |
| Membership | `Member` (slim), `Invitation` | `NewMemberService`, `MemberRegistrationService`, `AlumniGraduationService`, `GoogleMapCoordinateService` | `Pages/User/**`, `Pages/Map`, `Admin/Members`, `Admin/Invitations` |
| Billing | `MemberSubscription`, `MemberSubscriptionPlan`, `BillingActivity` | `WebhookHandlerService`, `Stripe*` handlers, renewal/cancellation/period-calc services, `CsvService` | `Pages/Checkout`, Stripe webhooks, `BillingEndpoints`, `MyProfile/Billing|Cancel`, `Admin/{SubscriptionPlans,ManageSubscriptions,UserReports,Members/Cancel}` |
| Videos | `ArchiveVideo`, `VideoComment`, `MemberVideoProgress`, `MemberFavoriteArchiveVideo` | `VideosService`, `CreateVideoService`, `VideoDetailsService`, `WebVTTParsingService` | `Pages/Videos`, `VideoEndpoints` (16), `Admin/{Videos,ArchivedVideos}` |
| Coaching | `CoachingSession`, `Question`, `QuestionVote` | (page-model logic only) | `Pages/CoachingSessions`, `Pages/Questions`, `CoachingSessionEndpoints` |
| Books | `Book`, `BookCategory`, (`MemberBookRead` new) | `BookService`, `LeaderboardService`, `RankingService`, `RankAndOrderService` | `Pages/{Books,Leaderboard}`, `Admin/Books`, `MyProfile/Books*` |
| Notifications & Ops | `DailyCheck` | email transports, Discord client, `BackgroundTaskQueue`, `DailyCheckService` | (none — transports only) |
| Host | — | Program.cs, layout, seeding, migration runner | Home/marketing, FAQ, CodeOfConduct, Calendar, Resources |
