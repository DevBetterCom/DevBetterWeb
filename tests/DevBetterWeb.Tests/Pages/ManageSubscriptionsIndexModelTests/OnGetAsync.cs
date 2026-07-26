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
