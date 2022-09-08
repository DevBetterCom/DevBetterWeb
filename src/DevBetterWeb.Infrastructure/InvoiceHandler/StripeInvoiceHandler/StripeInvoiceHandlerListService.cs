using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.InvoiceHandler.StripeInvoiceHandler;

public class StripeInvoiceHandlerListService : IInvoiceHandlerListService
{
	private readonly InvoiceService _invoiceService;

	public StripeInvoiceHandlerListService(InvoiceService invoiceService)
	{
		_invoiceService = invoiceService;
	}

  public async Task<List<Invoice>> ListAsync(CancellationToken cancellationToken = default)
  {
	  var invoiceListOptions = new InvoiceListOptions { Limit = 100 };

	  var stripeListInvoices = new StripeList<Invoice> { HasMore = true };
		var invoices = new List<Invoice>();

	  while (stripeListInvoices.HasMore)
	  {
		  stripeListInvoices = await _invoiceService.ListAsync(invoiceListOptions, cancellationToken: cancellationToken);
		  invoices.AddRange(stripeListInvoices.Data);
		}

	  return invoices;
  }

  public async Task<List<Invoice>> ListByEmailAsync(string memberEmail, CancellationToken cancellationToken = default)
  {
	  var invoiceListOptions = new InvoiceListOptions { Limit = 100 };

	  var stripeListInvoices = new StripeList<Invoice> { HasMore = true };
	  var invoices = new List<Invoice>();

	  while (stripeListInvoices.HasMore)
	  {
		  stripeListInvoices = await _invoiceService.ListAsync(invoiceListOptions, cancellationToken: cancellationToken);
		  var customerInvoices = stripeListInvoices.Data.Where(i => string.Equals(i.CustomerEmail, memberEmail, StringComparison.CurrentCultureIgnoreCase)).ToList();
		  if (customerInvoices.Count > 0)
		  {
			  invoices.AddRange(customerInvoices);
		  }
	  }

	  return invoices;
  }
  
  public async Task<List<Invoice>> SearchByEmailAsync(string memberEmail, CancellationToken cancellationToken = default)
  {
	  var invoiceSearchOptions = new InvoiceSearchOptions();
	  invoiceSearchOptions.Query = $"metadata['customer_email']:'{memberEmail}'";
	  invoiceSearchOptions.Limit = 100;

	  var stripeSearchInvoices = new StripeSearchResult<Invoice>{ HasMore = true };
	  var invoices = new List<Invoice>();

	  while (stripeSearchInvoices.HasMore)
	  {
		  stripeSearchInvoices = await _invoiceService.SearchAsync(invoiceSearchOptions, cancellationToken: cancellationToken);
		  invoiceSearchOptions.Page = stripeSearchInvoices.NextPage;

		  if (stripeSearchInvoices.Data.Count > 0)
		  {
			  invoices.AddRange(stripeSearchInvoices.Data);
		  }
	  }

	  return invoices;
  }
}
