using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Interfaces;
using Stripe;
using Stripe.Issuing;

namespace DevBetterWeb.Infrastructure.IssuingHandler.StripeIssuingHandler;

public class StripeIssuingHandlerTransactionListService : IIssuingHandlerTransactionListService
{
	private readonly TransactionService _transactionService;
	private readonly IIssuingHandlerCardListService _issuingHandlerCardListService;

	public StripeIssuingHandlerTransactionListService(TransactionService transactionService, IIssuingHandlerCardListService issuingHandlerCardListService)
	{
		_transactionService = transactionService;
		_issuingHandlerCardListService = issuingHandlerCardListService;
	}

  public async Task<List<Transaction>> ListAsync(CancellationToken cancellationToken = default)
  {
	  var transactionListOptions = new TransactionListOptions { Limit = 100 };

	  var transactions = new List<Transaction>();
	  var stripeListTransactions = new StripeList<Transaction> { HasMore = true };

	  while (stripeListTransactions.HasMore)
	  {
			stripeListTransactions = await _transactionService.ListAsync(transactionListOptions, cancellationToken: cancellationToken);
			transactions.AddRange(stripeListTransactions.Data);
		}
		

		return transactions;
  }

  public async Task<List<Transaction>> ListByEmailAsync(string memberEmail, CancellationToken cancellationToken = default)
  {
		var cards = await _issuingHandlerCardListService.ListByEmailAsync(memberEmail, cancellationToken);

		var transactions = new List<Transaction>();
		foreach (var card in cards)
		{
			var stripeListTransactions = new StripeList<Transaction> { HasMore = true };
			var transactionListOptions = new TransactionListOptions { Limit = 100, Cardholder = card.Cardholder.Name};
			while (stripeListTransactions.HasMore)
			{
				stripeListTransactions = await _transactionService.ListAsync(transactionListOptions, cancellationToken: cancellationToken);
				transactions.AddRange(stripeListTransactions.Data);
			}
		}

		return transactions;
  }
}
