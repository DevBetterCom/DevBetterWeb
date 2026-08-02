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
