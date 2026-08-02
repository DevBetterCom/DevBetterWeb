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
