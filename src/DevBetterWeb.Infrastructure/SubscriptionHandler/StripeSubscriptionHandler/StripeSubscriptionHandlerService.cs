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
		} while (page.HasMore && page.Data.Count > 0);

		return subscriptions.Where(s => _billableStatuses.Contains(s.Status)).ToList();
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
