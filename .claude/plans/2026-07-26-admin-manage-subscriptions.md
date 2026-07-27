# Admin Manage Subscriptions Page Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** A single admin-only "Manage Subscriptions" Razor Page that lists all billable Stripe subscriptions and lets an admin pause, resume, or cancel any one of them by subscription ID.

**Architecture:** Mirror the existing `IInvoiceHandlerListService` / `StripeInvoiceHandlerListService` pattern: a thin Infrastructure service wrapping Stripe.net's `SubscriptionService`, exposed through an interface in `DevBetterWeb.Infrastructure.Interfaces` that returns Stripe SDK types. The Web layer maps those to a `StripeSubscriptionDto` via AutoMapper (same as `InvoiceProfile`/`StripeInvoiceDto`) and renders them in a new page under `Pages/Admin/ManageSubscriptions/` with POST handlers per action. All actions target a specific subscription ID — deliberately unlike the existing `CancelSubscriptionAtPeriodEnd(email)`, which only resolves one subscription per email and misses duplicates.

**Tech Stack:** ASP.NET Core Razor Pages, Stripe.net 51.1.0, AutoMapper 16.1.1, xUnit + NSubstitute (Stripe.net service methods are `virtual`, so concrete `SubscriptionService` can be substituted directly).

## Global Constraints

- Per CLAUDE.md: before every commit, run the Codex review via `/codex:review` and fix or explicitly rebut findings.
- Stripe.net 51.x: `CurrentPeriodEnd` lives on `SubscriptionItem` (`subscription.Items.Data[0].CurrentPeriodEnd`), NOT on `Subscription`. See existing usage in `src/DevBetterWeb.Infrastructure/PaymentHandler/StripePaymentHandler/StripePaymentHandlerSubscriptionService.cs:182-194`.
- Do NOT copy the pagination loop from `StripeInvoiceHandlerListService.ListAsync` — it never advances `StartingAfter` (infinite-loop bug when >100 records). Use the corrected loop shown in Task 1.
- Admin authorization: `[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]` (same as `Pages/Admin/User.cshtml.cs`).
- Indentation: tabs in Web endpoints/Infrastructure files, 2 spaces in most page models — match whichever file you touch; new files use tabs like `StripeInvoiceHandlerListService.cs`.
- Test project: `tests/DevBetterWeb.Tests` (xUnit + NSubstitute). Run with `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~<name>"`.
- Money display: Stripe amounts are in cents; divide by 100 for display (see `Pages/Admin/User.cshtml:277`).

## File Structure

- Create: `src/DevBetterWeb.Infrastructure/Interfaces/ISubscriptionHandlerService.cs` — interface (List/Pause/Resume/Cancel)
- Create: `src/DevBetterWeb.Infrastructure/SubscriptionHandler/StripeSubscriptionHandler/StripeSubscriptionHandlerService.cs` — Stripe implementation
- Modify: `src/DevBetterWeb.Infrastructure/InfrastructureServiceCollectionExtensions.cs` — DI registration
- Create: `src/DevBetterWeb.Web/Models/StripeSubscriptionDto.cs` — web-layer DTO
- Create: `src/DevBetterWeb.Web/MappingProfiles/SubscriptionProfile.cs` — AutoMapper profile (auto-discovered via `AddMaps` in `Program.cs:106`)
- Create: `src/DevBetterWeb.Web/Pages/Admin/ManageSubscriptions/Index.cshtml` + `Index.cshtml.cs` — the page
- Modify: `src/DevBetterWeb.Web/Pages/Admin/Index.cshtml` — menu link
- Create: `tests/DevBetterWeb.Tests/Services/StripeSubscriptionHandlerServiceTests/*.cs`
- Create: `tests/DevBetterWeb.Tests/MappingProfiles/SubscriptionProfileTests.cs`
- Create: `tests/DevBetterWeb.Tests/Pages/ManageSubscriptionsIndexModelTests/*.cs`

---

### Task 1: `ISubscriptionHandlerService` + `ListBillableAsync`

**Files:**
- Create: `src/DevBetterWeb.Infrastructure/Interfaces/ISubscriptionHandlerService.cs`
- Create: `src/DevBetterWeb.Infrastructure/SubscriptionHandler/StripeSubscriptionHandler/StripeSubscriptionHandlerService.cs`
- Modify: `src/DevBetterWeb.Infrastructure/InfrastructureServiceCollectionExtensions.cs`
- Test: `tests/DevBetterWeb.Tests/Services/StripeSubscriptionHandlerServiceTests/ListBillableAsync.cs`

**Interfaces:**
- Consumes: Stripe.net `SubscriptionService` (already registered transient in DI at `InfrastructureServiceCollectionExtensions.cs:65`)
- Produces: `ISubscriptionHandlerService.ListBillableAsync(CancellationToken) → Task<List<Stripe.Subscription>>` (customer expanded on each subscription); registered in DI as `ISubscriptionHandlerService → StripeSubscriptionHandlerService`

- [ ] **Step 1: Write the failing test**

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.SubscriptionHandler.StripeSubscriptionHandler;
using NSubstitute;
using Stripe;
using Xunit;

namespace DevBetterWeb.Tests.Services.StripeSubscriptionHandlerServiceTests;

public class ListBillableAsync
{
	private readonly SubscriptionService _stripeSubscriptionService = Substitute.For<SubscriptionService>();

	[Fact]
	public async Task ReturnsOnlyBillableStatusesAndExcludesCanceled()
	{
		var page = new StripeList<Subscription>
		{
			HasMore = false,
			Data = new List<Subscription>
			{
				new Subscription { Id = "sub_active", Status = "active" },
				new Subscription { Id = "sub_pastdue", Status = "past_due" },
				new Subscription { Id = "sub_canceled", Status = "canceled" },
				new Subscription { Id = "sub_incomplete", Status = "incomplete_expired" },
			},
		};
		_stripeSubscriptionService
			.ListAsync(Arg.Any<SubscriptionListOptions>(), Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>())
			.Returns(page);
		var service = new StripeSubscriptionHandlerService(_stripeSubscriptionService);

		var result = await service.ListBillableAsync();

		Assert.Equal(2, result.Count);
		Assert.Contains(result, s => s.Id == "sub_active");
		Assert.Contains(result, s => s.Id == "sub_pastdue");
		Assert.DoesNotContain(result, s => s.Id == "sub_canceled");
	}

	[Fact]
	public async Task RequestsAllStatusesWithCustomerExpanded()
	{
		_stripeSubscriptionService
			.ListAsync(Arg.Any<SubscriptionListOptions>(), Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>())
			.Returns(new StripeList<Subscription> { HasMore = false, Data = new List<Subscription>() });
		var service = new StripeSubscriptionHandlerService(_stripeSubscriptionService);

		await service.ListBillableAsync();

		await _stripeSubscriptionService.Received(1).ListAsync(
			Arg.Is<SubscriptionListOptions>(o => o.Status == "all" && o.Expand.Contains("data.customer")),
			Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>());
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~StripeSubscriptionHandlerServiceTests"`
Expected: FAIL — does not compile: `StripeSubscriptionHandlerService` not defined.

- [ ] **Step 3: Write the interface and minimal implementation**

`src/DevBetterWeb.Infrastructure/Interfaces/ISubscriptionHandlerService.cs`:

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stripe;

namespace DevBetterWeb.Infrastructure.Interfaces;

public interface ISubscriptionHandlerService
{
	Task<List<Subscription>> ListBillableAsync(CancellationToken cancellationToken = default);
	Task<Subscription> PauseAsync(string subscriptionId, CancellationToken cancellationToken = default);
	Task<Subscription> ResumeAsync(string subscriptionId, CancellationToken cancellationToken = default);
	Task<Subscription> CancelAtPeriodEndAsync(string subscriptionId, CancellationToken cancellationToken = default);
	Task<Subscription> CancelImmediatelyAsync(string subscriptionId, CancellationToken cancellationToken = default);
}
```

`src/DevBetterWeb.Infrastructure/SubscriptionHandler/StripeSubscriptionHandler/StripeSubscriptionHandlerService.cs` (Pause/Resume/Cancel throw `NotImplementedException` for now — implemented in Tasks 2-3):

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.SubscriptionHandler.StripeSubscriptionHandler;

public class StripeSubscriptionHandlerService : ISubscriptionHandlerService
{
	private static readonly HashSet<string> _billableStatuses =
		new() { "active", "past_due", "trialing", "unpaid" };

	private readonly SubscriptionService _subscriptionService;

	public StripeSubscriptionHandlerService(SubscriptionService subscriptionService)
	{
		_subscriptionService = subscriptionService;
	}

	public async Task<List<Subscription>> ListBillableAsync(CancellationToken cancellationToken = default)
	{
		var options = new SubscriptionListOptions { Status = "all", Limit = 100 };
		options.AddExpand("data.customer");

		var subscriptions = new List<Subscription>();
		StripeList<Subscription> page;
		do
		{
			page = await _subscriptionService.ListAsync(options, cancellationToken: cancellationToken);
			subscriptions.AddRange(page.Data);
			if (page.Data.Count > 0)
			{
				options.StartingAfter = page.Data[^1].Id;
			}
		} while (page.HasMore);

		return subscriptions.Where(s => _billableStatuses.Contains(s.Status)).ToList();
	}

	public Task<Subscription> PauseAsync(string subscriptionId, CancellationToken cancellationToken = default)
		=> throw new NotImplementedException();

	public Task<Subscription> ResumeAsync(string subscriptionId, CancellationToken cancellationToken = default)
		=> throw new NotImplementedException();

	public Task<Subscription> CancelAtPeriodEndAsync(string subscriptionId, CancellationToken cancellationToken = default)
		=> throw new NotImplementedException();

	public Task<Subscription> CancelImmediatelyAsync(string subscriptionId, CancellationToken cancellationToken = default)
		=> throw new NotImplementedException();
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~StripeSubscriptionHandlerServiceTests"`
Expected: PASS (both tests).

- [ ] **Step 5: Register in DI**

In `src/DevBetterWeb.Infrastructure/InfrastructureServiceCollectionExtensions.cs`, add after line 76 (`services.AddTransient<IInvoiceHandlerListService, StripeInvoiceHandlerListService>();`):

```csharp
		services.AddTransient<ISubscriptionHandlerService, StripeSubscriptionHandlerService>();
```

And add the using at the top with the other Infrastructure usings:

```csharp
using DevBetterWeb.Infrastructure.SubscriptionHandler.StripeSubscriptionHandler;
```

- [ ] **Step 6: Build the solution**

Run: `dotnet build DevBetterWeb.sln`
Expected: Build succeeded.

- [ ] **Step 7: Codex review, then commit**

Run `/codex:review`; fix or rebut findings. Then:

```bash
git add src/DevBetterWeb.Infrastructure tests/DevBetterWeb.Tests
git commit -m "feat: add ISubscriptionHandlerService with billable subscription listing"
```

---

### Task 2: Pause and Resume

**Files:**
- Modify: `src/DevBetterWeb.Infrastructure/SubscriptionHandler/StripeSubscriptionHandler/StripeSubscriptionHandlerService.cs`
- Test: `tests/DevBetterWeb.Tests/Services/StripeSubscriptionHandlerServiceTests/PauseAndResumeAsync.cs`

**Interfaces:**
- Consumes: `StripeSubscriptionHandlerService` from Task 1 (replaces the two `NotImplementedException` bodies for `PauseAsync`/`ResumeAsync`)
- Produces: `PauseAsync(string subscriptionId, CancellationToken) → Task<Subscription>` — sets `pause_collection.behavior = "void"` (Stripe stops charging; invoices generated while paused are voided). `ResumeAsync(string subscriptionId, CancellationToken) → Task<Subscription>` — clears `pause_collection`.

- [ ] **Step 1: Write the failing tests**

```csharp
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.SubscriptionHandler.StripeSubscriptionHandler;
using NSubstitute;
using Stripe;
using Xunit;

namespace DevBetterWeb.Tests.Services.StripeSubscriptionHandlerServiceTests;

public class PauseAndResumeAsync
{
	private const string SubscriptionId = "sub_123";
	private readonly SubscriptionService _stripeSubscriptionService = Substitute.For<SubscriptionService>();
	private readonly StripeSubscriptionHandlerService _service;

	public PauseAndResumeAsync()
	{
		_stripeSubscriptionService
			.UpdateAsync(SubscriptionId, Arg.Any<SubscriptionUpdateOptions>(), Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>())
			.Returns(new Subscription { Id = SubscriptionId });
		_service = new StripeSubscriptionHandlerService(_stripeSubscriptionService);
	}

	[Fact]
	public async Task PauseUpdatesSubscriptionWithVoidPauseBehavior()
	{
		await _service.PauseAsync(SubscriptionId);

		await _stripeSubscriptionService.Received(1).UpdateAsync(SubscriptionId,
			Arg.Is<SubscriptionUpdateOptions>(o =>
				o.PauseCollection != null && o.PauseCollection.Behavior == "void"),
			Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ResumeUpdatesSubscriptionClearingPauseCollection()
	{
		await _service.ResumeAsync(SubscriptionId);

		// Clearing pause_collection requires sending an empty value; Stripe.net does this
		// via AddExtraParam, which lands in the options' ExtraParams dictionary.
		await _stripeSubscriptionService.Received(1).UpdateAsync(SubscriptionId,
			Arg.Is<SubscriptionUpdateOptions>(o =>
				o.ExtraParams != null && o.ExtraParams.ContainsKey("pause_collection")),
			Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>());
	}
}
```

Note: if `ExtraParams` is not publicly readable in Stripe.net 51.x, change the `Resume` assertion to `Arg.Is<SubscriptionUpdateOptions>(o => o.PauseCollection == null)` combined with `Received(1)` — the essential behavior is "an update call that does not set a pause".

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~PauseAndResumeAsync"`
Expected: FAIL with `NotImplementedException`.

- [ ] **Step 3: Implement Pause and Resume**

Replace the two stub methods in `StripeSubscriptionHandlerService`:

```csharp
	public Task<Subscription> PauseAsync(string subscriptionId, CancellationToken cancellationToken = default)
	{
		var options = new SubscriptionUpdateOptions
		{
			PauseCollection = new SubscriptionPauseCollectionOptions
			{
				Behavior = "void",
			},
		};

		return _subscriptionService.UpdateAsync(subscriptionId, options, cancellationToken: cancellationToken);
	}

	public Task<Subscription> ResumeAsync(string subscriptionId, CancellationToken cancellationToken = default)
	{
		var options = new SubscriptionUpdateOptions();
		options.AddExtraParam("pause_collection", "");

		return _subscriptionService.UpdateAsync(subscriptionId, options, cancellationToken: cancellationToken);
	}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~PauseAndResumeAsync"`
Expected: PASS.

- [ ] **Step 5: Codex review, then commit**

Run `/codex:review`; fix or rebut findings. Then:

```bash
git add src/DevBetterWeb.Infrastructure tests/DevBetterWeb.Tests
git commit -m "feat: pause and resume Stripe subscriptions by id"
```

---

### Task 3: Cancel (at period end and immediately)

**Files:**
- Modify: `src/DevBetterWeb.Infrastructure/SubscriptionHandler/StripeSubscriptionHandler/StripeSubscriptionHandlerService.cs`
- Test: `tests/DevBetterWeb.Tests/Services/StripeSubscriptionHandlerServiceTests/CancelAsync.cs`

**Interfaces:**
- Consumes: `StripeSubscriptionHandlerService` from Task 1 (replaces the remaining two `NotImplementedException` bodies)
- Produces: `CancelAtPeriodEndAsync(string subscriptionId, CancellationToken) → Task<Subscription>` — sets `CancelAtPeriodEnd = true` (member keeps access until period end; the existing `customer.subscription.deleted` webhook fires at period end and removes the Member role). `CancelImmediatelyAsync(string subscriptionId, CancellationToken) → Task<Subscription>` — calls Stripe cancel now (for killing duplicate subscriptions).

- [ ] **Step 1: Write the failing tests**

```csharp
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.SubscriptionHandler.StripeSubscriptionHandler;
using NSubstitute;
using Stripe;
using Xunit;

namespace DevBetterWeb.Tests.Services.StripeSubscriptionHandlerServiceTests;

public class CancelAsync
{
	private const string SubscriptionId = "sub_123";
	private readonly SubscriptionService _stripeSubscriptionService = Substitute.For<SubscriptionService>();
	private readonly StripeSubscriptionHandlerService _service;

	public CancelAsync()
	{
		_stripeSubscriptionService
			.UpdateAsync(SubscriptionId, Arg.Any<SubscriptionUpdateOptions>(), Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>())
			.Returns(new Subscription { Id = SubscriptionId });
		_stripeSubscriptionService
			.CancelAsync(SubscriptionId, Arg.Any<SubscriptionCancelOptions>(), Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>())
			.Returns(new Subscription { Id = SubscriptionId, Status = "canceled" });
		_service = new StripeSubscriptionHandlerService(_stripeSubscriptionService);
	}

	[Fact]
	public async Task CancelAtPeriodEndSetsFlagOnSubscription()
	{
		await _service.CancelAtPeriodEndAsync(SubscriptionId);

		await _stripeSubscriptionService.Received(1).UpdateAsync(SubscriptionId,
			Arg.Is<SubscriptionUpdateOptions>(o => o.CancelAtPeriodEnd == true),
			Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task CancelImmediatelyCallsStripeCancel()
	{
		var result = await _service.CancelImmediatelyAsync(SubscriptionId);

		Assert.Equal("canceled", result.Status);
		await _stripeSubscriptionService.Received(1).CancelAsync(SubscriptionId,
			Arg.Any<SubscriptionCancelOptions>(), Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>());
	}
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~StripeSubscriptionHandlerServiceTests.CancelAsync"`
Expected: FAIL with `NotImplementedException`.

- [ ] **Step 3: Implement both cancel methods**

Replace the remaining stubs in `StripeSubscriptionHandlerService`:

```csharp
	public Task<Subscription> CancelAtPeriodEndAsync(string subscriptionId, CancellationToken cancellationToken = default)
	{
		var options = new SubscriptionUpdateOptions
		{
			CancelAtPeriodEnd = true,
		};

		return _subscriptionService.UpdateAsync(subscriptionId, options, cancellationToken: cancellationToken);
	}

	public Task<Subscription> CancelImmediatelyAsync(string subscriptionId, CancellationToken cancellationToken = default)
	{
		return _subscriptionService.CancelAsync(subscriptionId, null, cancellationToken: cancellationToken);
	}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~StripeSubscriptionHandlerServiceTests"`
Expected: PASS (all Task 1-3 tests).

- [ ] **Step 5: Codex review, then commit**

Run `/codex:review`; fix or rebut findings. Then:

```bash
git add src/DevBetterWeb.Infrastructure tests/DevBetterWeb.Tests
git commit -m "feat: cancel Stripe subscriptions by id (at period end or immediately)"
```

---

### Task 4: `StripeSubscriptionDto` + AutoMapper profile

**Files:**
- Create: `src/DevBetterWeb.Web/Models/StripeSubscriptionDto.cs`
- Create: `src/DevBetterWeb.Web/MappingProfiles/SubscriptionProfile.cs`
- Test: `tests/DevBetterWeb.Tests/MappingProfiles/SubscriptionProfileTests.cs`

**Interfaces:**
- Consumes: `Stripe.Subscription` (with `Customer` expanded) from `ISubscriptionHandlerService.ListBillableAsync`
- Produces: `StripeSubscriptionDto` with properties `Id, Status, CustomerId, CustomerEmail, PlanName, Amount (decimal, dollars), Currency, Interval, CurrentPeriodEnd (DateTime), CancelAtPeriodEnd (bool), IsPaused (bool)`; AutoMapper map `Subscription → StripeSubscriptionDto` (profile auto-registered by `AddMaps` in `Program.cs:106`)

- [ ] **Step 1: Write the failing test**

```csharp
using System;
using System.Collections.Generic;
using AutoMapper;
using DevBetterWeb.Web.MappingProfiles;
using DevBetterWeb.Web.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Stripe;
using Xunit;

namespace DevBetterWeb.Tests.MappingProfiles;

public class SubscriptionProfileTests
{
	private readonly IMapper _mapper;

	public SubscriptionProfileTests()
	{
		var configuration = new MapperConfiguration(
			cfg => cfg.AddProfile<SubscriptionProfile>(),
			NullLoggerFactory.Instance);
		_mapper = configuration.CreateMapper();
	}

	[Fact]
	public void MapsSubscriptionToDto()
	{
		var subscription = new Subscription
		{
			Id = "sub_123",
			Status = "active",
			CustomerId = "cus_123",
			Customer = new Customer { Id = "cus_123", Email = "member@example.com" },
			CancelAtPeriodEnd = true,
			PauseCollection = new SubscriptionPauseCollection { Behavior = "void" },
			Items = new StripeList<SubscriptionItem>
			{
				Data = new List<SubscriptionItem>
				{
					new SubscriptionItem
					{
						CurrentPeriodEnd = new DateTime(2026, 8, 27, 0, 0, 0, DateTimeKind.Utc),
						Price = new Price
						{
							Nickname = "Monthly",
							UnitAmountDecimal = 20000m,
							Currency = "usd",
							Recurring = new PriceRecurring { Interval = "month" },
						},
					},
				},
			},
		};

		var dto = _mapper.Map<StripeSubscriptionDto>(subscription);

		Assert.Equal("sub_123", dto.Id);
		Assert.Equal("active", dto.Status);
		Assert.Equal("cus_123", dto.CustomerId);
		Assert.Equal("member@example.com", dto.CustomerEmail);
		Assert.Equal("Monthly", dto.PlanName);
		Assert.Equal(200m, dto.Amount);
		Assert.Equal("usd", dto.Currency);
		Assert.Equal("month", dto.Interval);
		Assert.Equal(new DateTime(2026, 8, 27, 0, 0, 0, DateTimeKind.Utc), dto.CurrentPeriodEnd);
		Assert.True(dto.CancelAtPeriodEnd);
		Assert.True(dto.IsPaused);
	}

	[Fact]
	public void MapsSubscriptionWithMissingOptionalDataWithoutThrowing()
	{
		var subscription = new Subscription { Id = "sub_bare", Status = "active" };

		var dto = _mapper.Map<StripeSubscriptionDto>(subscription);

		Assert.Equal("sub_bare", dto.Id);
		Assert.Equal(string.Empty, dto.CustomerEmail);
		Assert.False(dto.IsPaused);
		Assert.Equal(0m, dto.Amount);
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~SubscriptionProfileTests"`
Expected: FAIL — does not compile: `StripeSubscriptionDto` / `SubscriptionProfile` not defined.

- [ ] **Step 3: Write DTO and profile**

`src/DevBetterWeb.Web/Models/StripeSubscriptionDto.cs`:

```csharp
using System;

namespace DevBetterWeb.Web.Models;

public class StripeSubscriptionDto
{
	public string Id { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
	public string CustomerId { get; set; } = string.Empty;
	public string CustomerEmail { get; set; } = string.Empty;
	public string PlanName { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public string Currency { get; set; } = string.Empty;
	public string Interval { get; set; } = string.Empty;
	public DateTime CurrentPeriodEnd { get; set; }
	public bool CancelAtPeriodEnd { get; set; }
	public bool IsPaused { get; set; }
}
```

`src/DevBetterWeb.Web/MappingProfiles/SubscriptionProfile.cs` (the `(src, dest)` MapFrom overload takes a plain Func, so null-conditional operators are allowed — expression-tree overloads would reject them):

```csharp
using System.Linq;
using AutoMapper;
using DevBetterWeb.Web.Models;
using Stripe;

namespace DevBetterWeb.Web.MappingProfiles;

public class SubscriptionProfile : Profile
{
	public SubscriptionProfile()
	{
		CreateMap<Subscription, StripeSubscriptionDto>()
			.ForMember(dest => dest.CustomerEmail,
				opt => opt.MapFrom((src, _) => src.Customer?.Email ?? string.Empty))
			.ForMember(dest => dest.PlanName,
				opt => opt.MapFrom((src, _) => src.Items?.Data?.FirstOrDefault()?.Price?.Nickname ?? string.Empty))
			.ForMember(dest => dest.Amount,
				opt => opt.MapFrom((src, _) => (src.Items?.Data?.FirstOrDefault()?.Price?.UnitAmountDecimal ?? 0m) / 100m))
			.ForMember(dest => dest.Currency,
				opt => opt.MapFrom((src, _) => src.Items?.Data?.FirstOrDefault()?.Price?.Currency ?? string.Empty))
			.ForMember(dest => dest.Interval,
				opt => opt.MapFrom((src, _) => src.Items?.Data?.FirstOrDefault()?.Price?.Recurring?.Interval ?? string.Empty))
			.ForMember(dest => dest.CurrentPeriodEnd,
				opt => opt.MapFrom((src, _) => src.Items?.Data?.FirstOrDefault()?.CurrentPeriodEnd ?? default))
			.ForMember(dest => dest.IsPaused,
				opt => opt.MapFrom((src, _) => src.PauseCollection != null));
	}
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~SubscriptionProfileTests"`
Expected: PASS. (If `Customer`/`PauseCollection` setters are not public in Stripe.net 51.x, construct via the property `Customer = new Customer {...}` fails to compile — in that case assert only the settable properties in test 1 and keep test 2 as-is; note the deviation in the commit message.)

- [ ] **Step 5: Codex review, then commit**

Run `/codex:review`; fix or rebut findings. Then:

```bash
git add src/DevBetterWeb.Web tests/DevBetterWeb.Tests
git commit -m "feat: add StripeSubscriptionDto and AutoMapper profile"
```

---

### Task 5: Manage Subscriptions page — list view (GET)

**Files:**
- Create: `src/DevBetterWeb.Web/Pages/Admin/ManageSubscriptions/Index.cshtml.cs`
- Create: `src/DevBetterWeb.Web/Pages/Admin/ManageSubscriptions/Index.cshtml`
- Test: `tests/DevBetterWeb.Tests/Pages/ManageSubscriptionsIndexModelTests/OnGetAsync.cs`

**Interfaces:**
- Consumes: `ISubscriptionHandlerService.ListBillableAsync` (Task 1), AutoMapper map from Task 4
- Produces: `IndexModel` with `List<StripeSubscriptionDto> Subscriptions` property, `string? StatusMessage` ([TempData]), and `Task OnGetAsync()`; page routed at `/Admin/ManageSubscriptions`

- [ ] **Step 1: Write the failing test**

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.MappingProfiles;
using DevBetterWeb.Web.Pages.Admin.ManageSubscriptions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Stripe;
using Xunit;

namespace DevBetterWeb.Tests.Pages.ManageSubscriptionsIndexModelTests;

public class OnGetAsync
{
	private readonly ISubscriptionHandlerService _subscriptionHandlerService = Substitute.For<ISubscriptionHandlerService>();
	private readonly IndexModel _pageModel;

	public OnGetAsync()
	{
		var configuration = new MapperConfiguration(
			cfg => cfg.AddProfile<SubscriptionProfile>(),
			NullLoggerFactory.Instance);
		_pageModel = new IndexModel(_subscriptionHandlerService, configuration.CreateMapper());
		_pageModel.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
		{
			HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext(),
		};
	}

	[Fact]
	public async Task LoadsBillableSubscriptionsAsDtos()
	{
		_subscriptionHandlerService.ListBillableAsync(Arg.Any<CancellationToken>()).Returns(
			new List<Subscription>
			{
				new Subscription { Id = "sub_b", Status = "active" },
				new Subscription { Id = "sub_a", Status = "past_due" },
			});

		await _pageModel.OnGetAsync();

		Assert.Equal(2, _pageModel.Subscriptions.Count);
		Assert.Contains(_pageModel.Subscriptions, s => s.Id == "sub_a");
		Assert.Contains(_pageModel.Subscriptions, s => s.Id == "sub_b");
	}
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~ManageSubscriptionsIndexModelTests"`
Expected: FAIL — does not compile: `IndexModel` not defined.

- [ ] **Step 3: Write the page model (GET only; POST handlers come in Task 6)**

`src/DevBetterWeb.Web/Pages/Admin/ManageSubscriptions/Index.cshtml.cs`:

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Admin.ManageSubscriptions;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class IndexModel : PageModel
{
	private readonly ISubscriptionHandlerService _subscriptionHandlerService;
	private readonly IMapper _mapper;

	public List<StripeSubscriptionDto> Subscriptions { get; private set; } = new();

	[TempData]
	public string? StatusMessage { get; set; }

	public IndexModel(ISubscriptionHandlerService subscriptionHandlerService, IMapper mapper)
	{
		_subscriptionHandlerService = subscriptionHandlerService;
		_mapper = mapper;
	}

	public async Task OnGetAsync()
	{
		var subscriptions = await _subscriptionHandlerService.ListBillableAsync(HttpContext.RequestAborted);
		Subscriptions = _mapper.Map<List<StripeSubscriptionDto>>(subscriptions)
			.OrderBy(s => s.CustomerEmail)
			.ThenBy(s => s.Id)
			.ToList();
	}
}
```

Note: the page model reads `HttpContext.RequestAborted`, which is why the test constructor sets a `PageContext` with a `DefaultHttpContext` — a bare PageModel would throw `NullReferenceException`.

- [ ] **Step 4: Write the page markup**

`src/DevBetterWeb.Web/Pages/Admin/ManageSubscriptions/Index.cshtml` (table only in this task; action buttons in Task 6):

```html
@page
@model DevBetterWeb.Web.Pages.Admin.ManageSubscriptions.IndexModel

@{
	ViewData["Title"] = "Manage Subscriptions";
}

<h2>Manage Subscriptions</h2>

@if (!string.IsNullOrEmpty(Model.StatusMessage))
{
	<div class="alert alert-info">@Model.StatusMessage</div>
}

<div class="card">
	<div class="card-header">
		<b>Billable Stripe Subscriptions (@Model.Subscriptions.Count)</b>
	</div>
	<div class="card-body" style="overflow: auto;">
		<table class="table">
			<thead>
			<tr>
				<th>Customer Email</th>
				<th>Plan</th>
				<th>Amount</th>
				<th>Interval</th>
				<th>Status</th>
				<th>Current Period End</th>
				<th>Actions</th>
			</tr>
			</thead>
			<tbody>
			@foreach (var subscription in Model.Subscriptions)
			{
				<tr>
					<td>@subscription.CustomerEmail<br /><small class="text-muted">@subscription.Id</small></td>
					<td>@subscription.PlanName</td>
					<td>$@subscription.Amount.ToString("F2") @subscription.Currency.ToUpper()</td>
					<td>@subscription.Interval</td>
					<td>
						@subscription.Status
						@if (subscription.IsPaused)
						{
							<span class="badge badge-warning">Paused</span>
						}
						@if (subscription.CancelAtPeriodEnd)
						{
							<span class="badge badge-secondary">Cancels at period end</span>
						}
					</td>
					<td>@subscription.CurrentPeriodEnd.ToString("MM/dd/yyyy")</td>
					<td><!-- Action buttons added in Task 6 --></td>
				</tr>
			}
			</tbody>
		</table>
	</div>
</div>
```

- [ ] **Step 5: Run test to verify it passes, and build**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~ManageSubscriptionsIndexModelTests"`
Expected: PASS.
Run: `dotnet build DevBetterWeb.sln`
Expected: Build succeeded.

- [ ] **Step 6: Codex review, then commit**

Run `/codex:review`; fix or rebut findings. Then:

```bash
git add src/DevBetterWeb.Web tests/DevBetterWeb.Tests
git commit -m "feat: add admin Manage Subscriptions page listing billable subscriptions"
```

---

### Task 6: Pause / Resume / Cancel actions + admin menu link

**Files:**
- Modify: `src/DevBetterWeb.Web/Pages/Admin/ManageSubscriptions/Index.cshtml.cs`
- Modify: `src/DevBetterWeb.Web/Pages/Admin/ManageSubscriptions/Index.cshtml`
- Modify: `src/DevBetterWeb.Web/Pages/Admin/Index.cshtml`
- Test: `tests/DevBetterWeb.Tests/Pages/ManageSubscriptionsIndexModelTests/PostHandlers.cs`

**Interfaces:**
- Consumes: `ISubscriptionHandlerService.PauseAsync/ResumeAsync/CancelAtPeriodEndAsync/CancelImmediatelyAsync` (Tasks 2-3); `IndexModel` from Task 5
- Produces: `OnPostPauseAsync(string subscriptionId)`, `OnPostResumeAsync(string subscriptionId)`, `OnPostCancelAsync(string subscriptionId)`, `OnPostCancelNowAsync(string subscriptionId)` — each returns `Task<IActionResult>` (redirect back to the page, PRG pattern, outcome in `StatusMessage`)

- [ ] **Step 1: Write the failing tests**

```csharp
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.MappingProfiles;
using DevBetterWeb.Web.Pages.Admin.ManageSubscriptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Stripe;
using Xunit;

namespace DevBetterWeb.Tests.Pages.ManageSubscriptionsIndexModelTests;

public class PostHandlers
{
	private const string SubscriptionId = "sub_123";
	private readonly ISubscriptionHandlerService _subscriptionHandlerService = Substitute.For<ISubscriptionHandlerService>();
	private readonly IndexModel _pageModel;

	public PostHandlers()
	{
		var configuration = new MapperConfiguration(
			cfg => cfg.AddProfile<SubscriptionProfile>(),
			NullLoggerFactory.Instance);
		_pageModel = new IndexModel(_subscriptionHandlerService, configuration.CreateMapper());
		_pageModel.PageContext = new PageContext { HttpContext = new DefaultHttpContext() };
		_pageModel.TempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
	}

	[Fact]
	public async Task PausePausesSubscriptionAndRedirects()
	{
		var result = await _pageModel.OnPostPauseAsync(SubscriptionId);

		await _subscriptionHandlerService.Received(1).PauseAsync(SubscriptionId, Arg.Any<CancellationToken>());
		Assert.IsType<RedirectToPageResult>(result);
		Assert.Contains("paused", _pageModel.StatusMessage);
	}

	[Fact]
	public async Task ResumeResumesSubscriptionAndRedirects()
	{
		var result = await _pageModel.OnPostResumeAsync(SubscriptionId);

		await _subscriptionHandlerService.Received(1).ResumeAsync(SubscriptionId, Arg.Any<CancellationToken>());
		Assert.IsType<RedirectToPageResult>(result);
	}

	[Fact]
	public async Task CancelCancelsAtPeriodEndAndRedirects()
	{
		var result = await _pageModel.OnPostCancelAsync(SubscriptionId);

		await _subscriptionHandlerService.Received(1).CancelAtPeriodEndAsync(SubscriptionId, Arg.Any<CancellationToken>());
		Assert.IsType<RedirectToPageResult>(result);
	}

	[Fact]
	public async Task CancelNowCancelsImmediatelyAndRedirects()
	{
		var result = await _pageModel.OnPostCancelNowAsync(SubscriptionId);

		await _subscriptionHandlerService.Received(1).CancelImmediatelyAsync(SubscriptionId, Arg.Any<CancellationToken>());
		Assert.IsType<RedirectToPageResult>(result);
	}

	[Fact]
	public async Task StripeErrorIsReportedInStatusMessageNotThrown()
	{
		_subscriptionHandlerService.PauseAsync(SubscriptionId, Arg.Any<CancellationToken>())
			.Returns<Task<Subscription>>(_ => throw new StripeException("No such subscription"));

		var result = await _pageModel.OnPostPauseAsync(SubscriptionId);

		Assert.IsType<RedirectToPageResult>(result);
		Assert.Contains("No such subscription", _pageModel.StatusMessage);
	}

	[Fact]
	public async Task MissingSubscriptionIdIsRejectedWithoutCallingStripe()
	{
		var result = await _pageModel.OnPostPauseAsync("");

		await _subscriptionHandlerService.DidNotReceiveWithAnyArgs().PauseAsync(default!, default);
		Assert.IsType<RedirectToPageResult>(result);
	}
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~PostHandlers"`
Expected: FAIL — does not compile: `OnPostPauseAsync` etc. not defined.

- [ ] **Step 3: Implement the POST handlers**

Add to `IndexModel`:

```csharp
	public Task<IActionResult> OnPostPauseAsync(string subscriptionId)
		=> ExecuteActionAsync(subscriptionId, id => _subscriptionHandlerService.PauseAsync(id, HttpContext.RequestAborted),
			"paused (Stripe will not charge until resumed)");

	public Task<IActionResult> OnPostResumeAsync(string subscriptionId)
		=> ExecuteActionAsync(subscriptionId, id => _subscriptionHandlerService.ResumeAsync(id, HttpContext.RequestAborted),
			"resumed");

	public Task<IActionResult> OnPostCancelAsync(string subscriptionId)
		=> ExecuteActionAsync(subscriptionId, id => _subscriptionHandlerService.CancelAtPeriodEndAsync(id, HttpContext.RequestAborted),
			"set to cancel at period end");

	public Task<IActionResult> OnPostCancelNowAsync(string subscriptionId)
		=> ExecuteActionAsync(subscriptionId, id => _subscriptionHandlerService.CancelImmediatelyAsync(id, HttpContext.RequestAborted),
			"canceled immediately");

	private async Task<IActionResult> ExecuteActionAsync(string subscriptionId,
		System.Func<string, Task<Stripe.Subscription>> action, string successVerb)
	{
		if (string.IsNullOrWhiteSpace(subscriptionId))
		{
			StatusMessage = "No subscription id provided.";
			return RedirectToPage();
		}

		try
		{
			await action(subscriptionId);
			StatusMessage = $"Subscription {subscriptionId} {successVerb}.";
		}
		catch (Stripe.StripeException exception)
		{
			StatusMessage = $"Stripe error for {subscriptionId}: {exception.Message}";
		}

		return RedirectToPage();
	}
```

Add `using System;` if not already present (for `Func`), or keep the fully-qualified form shown above.

- [ ] **Step 4: Add the action buttons to the table**

Replace the `<td><!-- Action buttons added in Task 6 --></td>` cell in `Index.cshtml`:

```html
				<td style="white-space: nowrap;">
					@if (subscription.IsPaused)
					{
						<form method="post" asp-page-handler="Resume" style="display:inline">
							<input type="hidden" name="subscriptionId" value="@subscription.Id" />
							<button type="submit" class="btn btn-sm btn-success">Resume</button>
						</form>
					}
					else
					{
						<form method="post" asp-page-handler="Pause" style="display:inline"
						      onsubmit="return confirm('Pause collection for @subscription.CustomerEmail? Stripe will stop charging until resumed.');">
							<input type="hidden" name="subscriptionId" value="@subscription.Id" />
							<button type="submit" class="btn btn-sm btn-warning">Pause</button>
						</form>
					}
					@if (!subscription.CancelAtPeriodEnd)
					{
						<form method="post" asp-page-handler="Cancel" style="display:inline"
						      onsubmit="return confirm('Cancel @subscription.CustomerEmail at period end?');">
							<input type="hidden" name="subscriptionId" value="@subscription.Id" />
							<button type="submit" class="btn btn-sm btn-outline-danger">Cancel at Period End</button>
						</form>
					}
					<form method="post" asp-page-handler="CancelNow" style="display:inline"
					      onsubmit="return confirm('IMMEDIATELY cancel @subscription.Id for @subscription.CustomerEmail? This cannot be undone.');">
						<input type="hidden" name="subscriptionId" value="@subscription.Id" />
						<button type="submit" class="btn btn-sm btn-danger">Cancel Now</button>
					</form>
				</td>
```

- [ ] **Step 5: Add the admin menu link**

In `src/DevBetterWeb.Web/Pages/Admin/Index.cshtml`, add after the "Manage Subscription Plans" list item (lines 39-43):

```html
    <li class="list-group-item list-group-item-action">
        <a href="./Admin/ManageSubscriptions">
            <i class="fas fa-credit-card fa-fw" aria-hidden="true"></i> Manage Subscriptions
        </a>
    </li>
```

- [ ] **Step 6: Run all new tests and build**

Run: `dotnet test tests/DevBetterWeb.Tests/DevBetterWeb.Tests.csproj --filter "FullyQualifiedName~ManageSubscriptions | FullyQualifiedName~StripeSubscriptionHandlerServiceTests | FullyQualifiedName~SubscriptionProfileTests"`
Expected: PASS (all).
Run: `dotnet build DevBetterWeb.sln`
Expected: Build succeeded.

- [ ] **Step 7: Codex review, then commit**

Run `/codex:review`; fix or rebut findings. Then:

```bash
git add src/DevBetterWeb.Web tests/DevBetterWeb.Tests
git commit -m "feat: pause, resume, and cancel actions on Manage Subscriptions page"
```

---

### Task 7: Full-suite verification

**Files:** none new.

- [ ] **Step 1: Run the entire test suite**

Run: `dotnet test DevBetterWeb.sln`
Expected: all tests pass (pre-existing failures, if any, must be noted as pre-existing — verify against `git stash` / main if unsure).

- [ ] **Step 2: Manual smoke test (optional but recommended)**

Requires Stripe test-mode keys in user secrets / `appsettings.Testing.json`. Launch the site, sign in as an admin, open `/Admin/ManageSubscriptions`, and verify listing renders; pause + resume a test-clock subscription in Stripe test mode and confirm the badge toggles.

- [ ] **Step 3: Codex review of the full diff, then final commit if anything changed**

Run `/codex:review`; fix or rebut findings; commit any fixes.

## Design Notes (for the reviewer, not tasks)

- **Why pause `behavior: "void"`:** Stripe keeps the subscription and its billing anchor but voids invoices while paused — the safest "stop charging, keep the subscription" semantics. `keep_as_draft`/`mark_uncollectible` would accumulate invoices an admin must clean up.
- **Why actions take a subscription ID, not an email:** the existing `CancelSubscriptionAtPeriodEnd(email)` (`StripePaymentHandlerSubscriptionService.cs:35-45`) resolves at most one subscription per email; customers with duplicate active subscriptions (a real production incident) need per-subscription control.
- **Webhooks:** no new webhooks needed. `Cancel at period end` triggers the existing `customer.subscription.updated` → future-cancellation email; actual end triggers `customer.subscription.deleted` → member role removal. `Cancel Now` triggers `customer.subscription.deleted` immediately. Pausing emits `customer.subscription.updated`, which `CustomerSubscriptionUpdatedWebHook` may process — verify it tolerates a pause-only update (it only reacts to `cancel_at_period_end`, so it should no-op).
- **Not included (YAGNI):** filtering/search on the page, refunds (do refunds in the Stripe Dashboard), pausing with resume-at dates, and syncing the local `MemberSubscription` records — pause/cancel effects flow through existing webhooks.
