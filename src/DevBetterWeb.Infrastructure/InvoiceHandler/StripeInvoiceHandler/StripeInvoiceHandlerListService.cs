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
	private readonly IPaymentHandlerCustomerService _paymentHandlerCustomerService;

	public StripeInvoiceHandlerListService(InvoiceService invoiceService, IPaymentHandlerCustomerService paymentHandlerCustomerService)
	{
		_invoiceService = invoiceService;
		_paymentHandlerCustomerService = paymentHandlerCustomerService;
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
		var invoices = new List<Invoice>();

		var customer = _paymentHandlerCustomerService.GetCustomerByEmail(memberEmail);
		if (customer == null)
		{
			return invoices;
		}

		var invoiceSearchOptions = new InvoiceSearchOptions();
		invoiceSearchOptions.Query = $"customer:\"{customer.CustomerId}\"";
		invoiceSearchOptions.Limit = 100;

	  var stripeSearchInvoices = new StripeSearchResult<Invoice>{ HasMore = true };
	  
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

	public List<Invoice> SearchByEmail(string memberEmail)
	{
		var invoices = new List<Invoice>();

		var customer = _paymentHandlerCustomerService.GetCustomerByEmail(memberEmail);
		if (customer == null)
		{
			return invoices;
		}

		var invoiceSearchOptions = new InvoiceSearchOptions();
		invoiceSearchOptions.Query = $"customer:\"{customer.CustomerId}\"";
		invoiceSearchOptions.Limit = 100;

		var stripeSearchInvoices = new StripeSearchResult<Invoice> { HasMore = true };

		while (stripeSearchInvoices.HasMore)
		{
			stripeSearchInvoices = _invoiceService.Search(invoiceSearchOptions);
			invoiceSearchOptions.Page = stripeSearchInvoices.NextPage;

			if (stripeSearchInvoices.Data.Count > 0)
			{
				invoices.AddRange(stripeSearchInvoices.Data);
			}
		}

		return invoices;
	}
}
