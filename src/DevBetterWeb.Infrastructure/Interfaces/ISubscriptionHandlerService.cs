using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stripe;

namespace DevBetterWeb.Infrastructure.Interfaces;

public interface ISubscriptionHandlerService
{
	Task<List<Subscription>> ListBillableAsync(CancellationToken cancellationToken = default);
	Task<Subscription> PauseAsync(string subscriptionId, CancellationToken cancellationToken = default);
	Task<Subscription> ResumeAsync(string subscriptionId, CancellationToken cancellationToken = default);
	Task<Subscription> CancelAtPeriodEndAsync(string subscriptionId, CancellationToken cancellationToken = default);
	Task<Subscription> CancelImmediatelyAsync(string subscriptionId, CancellationToken cancellationToken = default);
}
