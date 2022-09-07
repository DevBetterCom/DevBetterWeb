using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stripe;

namespace DevBetterWeb.Infrastructure.Interfaces;

public interface IInvoiceHandlerListService
{
	Task<List<Invoice>> ListAsync(CancellationToken cancellationToken = default);
	Task<List<Invoice>> ListByEmailAsync(string memberEmail, CancellationToken cancellationToken = default);
}
