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
		// Provide subscriptions in non-sorted order; include same-email pair for ThenBy(Id) tie-break
		_subscriptionHandlerService.ListBillableAsync(Arg.Any<CancellationToken>()).Returns(
			new List<Subscription>
			{
				new Subscription
				{
					Id = "sub_c",
					Status = "active",
					Customer = new Customer { Email = "charlie@example.com" }
				},
				new Subscription
				{
					Id = "sub_a",
					Status = "active",
					Customer = new Customer { Email = "alice@example.com" }
				},
				new Subscription
				{
					Id = "sub_b",
					Status = "past_due",
					Customer = new Customer { Email = "alice@example.com" }
				},
			});

		await _pageModel.OnGetAsync();

		// Verify all three loaded and sorted by CustomerEmail then Id
		Assert.Equal(3, _pageModel.Subscriptions.Count);

		// First result: alice@example.com, sub_a (sorts before sub_b because a < b)
		Assert.Equal("alice@example.com", _pageModel.Subscriptions[0].CustomerEmail);
		Assert.Equal("sub_a", _pageModel.Subscriptions[0].Id);

		// Second result: alice@example.com, sub_b (same email, sorts by ID)
		Assert.Equal("alice@example.com", _pageModel.Subscriptions[1].CustomerEmail);
		Assert.Equal("sub_b", _pageModel.Subscriptions[1].Id);

		// Third result: charlie@example.com, sub_c (sorts after alice emails)
		Assert.Equal("charlie@example.com", _pageModel.Subscriptions[2].CustomerEmail);
		Assert.Equal("sub_c", _pageModel.Subscriptions[2].Id);
	}
}
