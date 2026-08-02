using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.SubscriptionHandler.StripeSubscriptionHandler;
using NSubstitute;
using Stripe;
using Xunit;

namespace DevBetterWeb.Tests.Services.StripeSubscriptionHandlerServiceTests;

public class ListBillableAsync
{
	private static readonly string[] _billableStatuses = { "active", "past_due", "trialing", "unpaid" };

	private readonly SubscriptionService _stripeSubscriptionService = Substitute.For<SubscriptionService>();

	private static StripeList<Subscription> EmptyPage() =>
		new() { HasMore = false, Data = new List<Subscription>() };

	[Fact]
	public async Task QueriesEachBillableStatusServerSideInsteadOfAll()
	{
		_stripeSubscriptionService
			.ListAsync(Arg.Any<SubscriptionListOptions>(), Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>())
			.Returns(EmptyPage());
		var service = new StripeSubscriptionHandlerService(_stripeSubscriptionService);

		await service.ListBillableAsync();

		foreach (var status in _billableStatuses)
		{
			await _stripeSubscriptionService.Received(1).ListAsync(
				Arg.Is<SubscriptionListOptions>(o => o.Status == status && o.Expand.Contains("data.customer")),
				Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>());
		}
		await _stripeSubscriptionService.DidNotReceive().ListAsync(
			Arg.Is<SubscriptionListOptions>(o => o.Status == "all"),
			Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ReturnsSubscriptionsFromAllBillableStatuses()
	{
		_stripeSubscriptionService
			.ListAsync(Arg.Any<SubscriptionListOptions>(), Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>())
			.Returns(callInfo =>
			{
				var options = callInfo.Arg<SubscriptionListOptions>();
				var data = options.Status switch
				{
					"active" => new List<Subscription> { new() { Id = "sub_active", Status = "active" } },
					"past_due" => new List<Subscription> { new() { Id = "sub_pastdue", Status = "past_due" } },
					_ => new List<Subscription>(),
				};
				return new StripeList<Subscription> { HasMore = false, Data = data };
			});
		var service = new StripeSubscriptionHandlerService(_stripeSubscriptionService);

		var result = await service.ListBillableAsync();

		Assert.Equal(2, result.Count);
		Assert.Contains(result, s => s.Id == "sub_active");
		Assert.Contains(result, s => s.Id == "sub_pastdue");
	}

	[Fact]
	public async Task PaginatesWithinAStatusUntilHasMoreIsFalse()
	{
		var firstPage = new StripeList<Subscription>
		{
			HasMore = true,
			Data = new List<Subscription> { new() { Id = "sub_1", Status = "active" } },
		};
		var secondPage = new StripeList<Subscription>
		{
			HasMore = false,
			Data = new List<Subscription> { new() { Id = "sub_2", Status = "active" } },
		};
		// Capture StartingAfter at call time: the service mutates one options
		// instance across pages, so Received() would only see the final value.
		var activeStartingAfterValues = new List<string?>();
		_stripeSubscriptionService
			.ListAsync(Arg.Any<SubscriptionListOptions>(), Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>())
			.Returns(callInfo =>
			{
				var options = callInfo.Arg<SubscriptionListOptions>();
				if (options.Status != "active")
				{
					return EmptyPage();
				}
				activeStartingAfterValues.Add(options.StartingAfter);
				return options.StartingAfter == null ? firstPage : secondPage;
			});
		var service = new StripeSubscriptionHandlerService(_stripeSubscriptionService);

		var result = await service.ListBillableAsync();

		Assert.Equal(2, result.Count);
		Assert.Contains(result, s => s.Id == "sub_1");
		Assert.Contains(result, s => s.Id == "sub_2");
		Assert.Equal(new List<string?> { null, "sub_1" }, activeStartingAfterValues);
	}
}
