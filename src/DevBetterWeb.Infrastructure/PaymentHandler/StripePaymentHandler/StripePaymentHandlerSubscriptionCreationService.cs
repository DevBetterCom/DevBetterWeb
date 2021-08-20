using System.Collections.Generic;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Interfaces;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler
{
  public class StripePaymentHandlerSubscriptionCreationService : IPaymentHandlerSubscriptionCreationService
  {
    private readonly SubscriptionService _subscriptionService;
    private readonly IPaymentHandlerPaymentMethod _paymentHandlerPaymentMethod;
    private readonly IPaymentHandlerCustomerService _paymentHandlerCustomer;

    public StripePaymentHandlerSubscriptionCreationService(SubscriptionService subscriptionService,
      IPaymentHandlerPaymentMethod paymentHandlerPaymentMethod,
      IPaymentHandlerCustomerService paymentHandlerCustomer)
    {
      _subscriptionService = subscriptionService;
      _paymentHandlerPaymentMethod = paymentHandlerPaymentMethod;
      _paymentHandlerCustomer = paymentHandlerCustomer;
    }

    public IPaymentHandlerSubscriptionDTO SetUpSubscription(string customerId, string priceId, string paymentMethodId)
    {
      // attach payment method
      _paymentHandlerPaymentMethod.AttachPaymentMethodToCustomer(paymentMethodId!, customerId!);

      // update customer's default invoice payment method
      _paymentHandlerCustomer.SetPaymentMethodAsCustomerDefault(customerId!, paymentMethodId!);

      // create subscription
      var subscription = CreateSubscription(customerId, priceId);

      return subscription;
    }

    private IPaymentHandlerSubscriptionDTO CreateSubscription(string customerId, string priceId)
    {
      var subscriptionOptions = new SubscriptionCreateOptions
      {
        Customer = customerId,
        Items = new List<SubscriptionItemOptions>
        {
          new SubscriptionItemOptions
          {
            Price = priceId,
          },
        },
      };
      subscriptionOptions.AddExpand("latest_invoice.payment_intent");

      var subscription = _subscriptionService.Create(subscriptionOptions);

      var id = subscription.Id;
      var status = subscription.Status;
      var latestInvoicePaymentIntentStatus = subscription.LatestInvoice.PaymentIntent.Status;
      var latestInvoicePaymentIntentClientSecret = subscription.LatestInvoice.PaymentIntent.ClientSecret;

      var subscriptionDTO = new StripePaymentHandlerSubscriptionDTO(id, status, latestInvoicePaymentIntentStatus, latestInvoicePaymentIntentClientSecret);

      return subscriptionDTO;
    }
  }

}
