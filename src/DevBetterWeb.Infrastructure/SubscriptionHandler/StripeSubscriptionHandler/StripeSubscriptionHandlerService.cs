using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.SubscriptionHandler.StripeSubscriptionHandler;

public class StripeSubscriptionHandlerService : ISubscriptionHandlerService
{
	// Querying per status lets Stripe filter server-side; listing status "all" pages
	// through every historical (canceled) subscription in the account.
	private static readonly string[] _billableStatuses = { "active", "past_due", "trialing", "unpaid" };

	private readonly SubscriptionService _subscriptionService;

	public StripeSubscriptionHandlerService(SubscriptionService subscriptionService)
	{
		_subscriptionService = subscriptionService;
	}

	public async Task<List<Subscription>> ListBillableAsync(CancellationToken cancellationToken = default)
	{
		var subscriptions = new List<Subscription>();
		foreach (var status in _billableStatuses)
		{
			var options = new SubscriptionListOptions { Status = status, Limit = 100 };
			options.AddExpand("data.customer");

			StripeList<Subscription> page;
			do
			{
				page = await _subscriptionService.ListAsync(options, cancellationToken: cancellationToken);
				subscriptions.AddRange(page.Data);
				if (page.Data.Count > 0)
				{
					options.StartingAfter = page.Data[^1].Id;
				}
			} while (page.HasMore && page.Data.Count > 0);
		}

		return subscriptions;
	}

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
}
