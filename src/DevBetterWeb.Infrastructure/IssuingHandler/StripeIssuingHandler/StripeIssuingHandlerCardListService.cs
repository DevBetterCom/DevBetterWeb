using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Interfaces;
using Stripe;
using Card = Stripe.Issuing.Card;
using CardListOptions = Stripe.Issuing.CardListOptions;
using CardService = Stripe.Issuing.CardService;

namespace DevBetterWeb.Infrastructure.IssuingHandler.StripeIssuingHandler;

public class StripeIssuingHandlerCardListService : IIssuingHandlerCardListService
{
	private readonly CardService _cardService;

	public StripeIssuingHandlerCardListService(CardService cardService)
	{
		_cardService = cardService;
	}

  public async Task<List<Card>> ListAsync(CancellationToken cancellationToken = default)
  {
	  var cardListOptions = new CardListOptions { Limit = 100 };

	  var cards = new List<Card>();
	  var stripeListCards = new StripeList<Card> { HasMore = true };

	  while (stripeListCards.HasMore)
	  {
		  stripeListCards = await _cardService.ListAsync(cardListOptions, cancellationToken: cancellationToken);
		  cards.AddRange(stripeListCards.Data);
		}
		

		return cards;
  }

  public async Task<List<Card>> ListByEmailAsync(string memberEmail, CancellationToken cancellationToken = default)
  {
	  var cardListOptions = new CardListOptions { Limit = 100 };

	  var stripeListCards = new StripeList<Card> { HasMore = true };
	  var cards = new List<Card>();

	  while (stripeListCards.HasMore)
	  {
		  stripeListCards = await _cardService.ListAsync(cardListOptions, cancellationToken: cancellationToken);
		  var customerCards = stripeListCards.Data.Where(c => string.Equals(c.Cardholder.Email, memberEmail, StringComparison.CurrentCultureIgnoreCase)).ToList();
		  if (customerCards.Count > 0)
		  {
			  cards.AddRange(customerCards);
		  }
	  }

	  return cards;
  }
}
