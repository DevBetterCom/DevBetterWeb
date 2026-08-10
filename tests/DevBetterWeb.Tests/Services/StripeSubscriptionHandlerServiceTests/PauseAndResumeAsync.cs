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
				o!.PauseCollection != null && o.PauseCollection.Behavior == "void"),
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
				o!.ExtraParams != null && o.ExtraParams.ContainsKey("pause_collection")),
			Arg.Any<RequestOptions>(), Arg.Any<CancellationToken>());
	}
}
