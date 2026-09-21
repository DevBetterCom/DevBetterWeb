using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core.Interfaces;
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
	private readonly IWebhookHandlerService _webhookHandlerService = Substitute.For<IWebhookHandlerService>();
	private readonly IndexModel _pageModel;

	public PostHandlers()
	{
		var configuration = new MapperConfiguration(
			cfg => cfg.AddProfile<SubscriptionProfile>(),
			NullLoggerFactory.Instance);
		_pageModel = new IndexModel(_subscriptionHandlerService, _webhookHandlerService, configuration.CreateMapper());
		_pageModel.PageContext = new PageContext { HttpContext = new DefaultHttpContext() };
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
	public async Task ReplayPaidInvoiceReprocessesInvoiceAndReportsResult()
	{
		_webhookHandlerService.ReprocessPaidInvoiceAsync("in_123").Returns("Invoice in_123 processed as a new subscription (sub_123).");

		var result = await _pageModel.OnPostReplayPaidInvoiceAsync(" in_123 ");

		await _webhookHandlerService.Received(1).ReprocessPaidInvoiceAsync("in_123");
		Assert.IsType<RedirectToPageResult>(result);
		Assert.Contains("processed as a new subscription", _pageModel.StatusMessage);
	}

	[Fact]
	public async Task ReplayPaidInvoiceErrorIsReportedInStatusMessageNotThrown()
	{
		_webhookHandlerService.ReprocessPaidInvoiceAsync("in_123")
			.Returns<Task<string>>(_ => throw new StripeException("No such invoice"));

		var result = await _pageModel.OnPostReplayPaidInvoiceAsync("in_123");

		Assert.IsType<RedirectToPageResult>(result);
		Assert.Contains("No such invoice", _pageModel.StatusMessage);
	}

	[Fact]
	public async Task ReplayPaidInvoiceRejectsMissingInvoiceId()
	{
		var result = await _pageModel.OnPostReplayPaidInvoiceAsync("");

		await _webhookHandlerService.DidNotReceiveWithAnyArgs().ReprocessPaidInvoiceAsync(default!);
		Assert.IsType<RedirectToPageResult>(result);
	}

	[Fact]
	public async Task ReplaySubscriptionEndedProcessesSubscriptionAsEnded()
	{
		_webhookHandlerService.ReprocessSubscriptionEndedAsync(SubscriptionId).Returns($"Subscription {SubscriptionId} processed as ended.");

		var result = await _pageModel.OnPostReplaySubscriptionEndedAsync(SubscriptionId);

		await _webhookHandlerService.Received(1).ReprocessSubscriptionEndedAsync(SubscriptionId);
		Assert.IsType<RedirectToPageResult>(result);
		Assert.Contains("processed as ended", _pageModel.StatusMessage);
	}

	[Fact]
	public async Task MissingSubscriptionIdIsRejectedWithoutCallingStripe()
	{
		var result = await _pageModel.OnPostPauseAsync("");

		await _subscriptionHandlerService.DidNotReceiveWithAnyArgs().PauseAsync(default!, default);
		Assert.IsType<RedirectToPageResult>(result);
	}
}
