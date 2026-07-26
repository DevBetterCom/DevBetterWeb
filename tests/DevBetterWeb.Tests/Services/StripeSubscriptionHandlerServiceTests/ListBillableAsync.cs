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
