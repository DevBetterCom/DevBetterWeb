using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stripe.Issuing;

namespace DevBetterWeb.Infrastructure.Interfaces;

public interface IIssuingHandlerTransactionListService
{
	Task<List<Transaction>> ListAsync(CancellationToken cancellationToken = default);
	Task<List<Transaction>> ListByEmailAsync(string memberEmail, CancellationToken cancellationToken = default);
}
