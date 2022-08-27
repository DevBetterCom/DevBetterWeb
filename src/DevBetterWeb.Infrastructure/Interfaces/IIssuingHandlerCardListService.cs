using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stripe.Issuing;

namespace DevBetterWeb.Infrastructure.Interfaces;

public interface IIssuingHandlerCardListService
{
	Task<List<Card>> ListAsync(CancellationToken cancellationToken = default);
	Task<List<Card>> ListByEmailAsync(string memberEmail, CancellationToken cancellationToken = default);
}
