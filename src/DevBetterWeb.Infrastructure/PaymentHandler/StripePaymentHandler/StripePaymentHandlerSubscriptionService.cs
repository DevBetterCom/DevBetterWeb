using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Enums;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Core.ValueObjects;
using Stripe;

namespace DevBetterWeb.Infrastructure.PaymentHandler.StripePaymentHandler;

public class StripePaymentHandlerSubscriptionService : IPaymentHandlerSubscription
{
  private readonly SubscriptionService _subscriptionService;
  private readonly IPaymentHandlerSubscriptionCreationService _paymentHandlerSubscriptionCreationService;
  private readonly IRepository<Invitation> _invitationRepository;
  private readonly IAppLogger<StripePaymentHandlerSubscriptionService> _logger;

  public StripePaymentHandlerSubscriptionService(SubscriptionService subscriptionService,
    IPaymentHandlerSubscriptionCreationService paymentHandlerSubscriptionCreationService,
    IRepository<Invitation> invitationRepository,
    IAppLogger<StripePaymentHandlerSubscriptionService> logger)
  {
    _subscriptionService = subscriptionService;
    _paymentHandlerSubscriptionCreationService = paymentHandlerSubscriptionCreationService;
    _invitationRepository = invitationRepository;
    _logger = logger;
  }

  public async Task CancelSubscriptionAtPeriodEnd(string customerEmail)
  {
    var spec = new InactiveInvitationByEmailSpec(customerEmail);
    var invite = await _invitationRepository.GetBySpecAsync(spec);
    if (invite is null) throw new InvitationNotFoundException(customerEmail);
    var subscriptionId = invite.PaymentHandlerSubscriptionId;

    var subscriptionCancelOptions = new SubscriptionUpdateOptions
    {
      CancelAtPeriodEnd = true,
    };

    _subscriptionService.Update(subscriptionId, subscriptionCancelOptions);
  }

  public IPaymentHandlerSubscriptionDTO CreateSubscription(string customerId, string priceId, string paymentMethodId)
  {
    var dto = _paymentHandlerSubscriptionCreationService.SetUpSubscription(customerId, priceId, paymentMethodId);
    return dto;
  }

  public IPaymentHandlerSubscriptionDTO CreateSubscriptionError(string errorMessage)
  {
    var subscriptionError = new StripePaymentHandlerSubscriptionDTO(errorMessage);
    return subscriptionError;
  }

  public bool GetCancelAtPeriodEnd(string subscriptionId)
  {
    var subscription = GetSubscription(subscriptionId);

    var cancelAtPeriodEnd = subscription.CancelAtPeriodEnd;

    return cancelAtPeriodEnd;
  }

  public string GetCustomerId(string subscriptionId)
  {
    Guard.Against.NullOrEmpty(subscriptionId, nameof(subscriptionId));
    var subscription = GetSubscription(subscriptionId);

    var customerId = subscription.CustomerId;

    return customerId;
  }

  public decimal GetSubscriptionAmount(string subscriptionId)
  {
    Guard.Against.NullOrEmpty(subscriptionId, nameof(subscriptionId));
    var subscription = GetSubscription(subscriptionId);

    decimal? amount = subscription?.Items?.GetEnumerator()?.Current?.Price?.UnitAmountDecimal;

    return amount ?? 0;
  }

  public DateTimeRange GetDateTimeRange(string subscriptionId)
  {
    var subscription = GetSubscription(subscriptionId);

    var startDate = GetStartDate(subscription);
    var endDate = GetEndDate(subscription);

    var dateTimeRange = new DateTimeRange(startDate, endDate);

    return dateTimeRange;
  }

  public DateTime GetEndDate(string subscriptionId)
  {
    var subscription = GetSubscription(subscriptionId);

    var endDate = GetEndDate(subscription);

    return endDate;
  }

  public string GetStatus(string subscriptionId)
  {
    var subscription = GetSubscription(subscriptionId);

    var status = subscription.Status;

    return status;
  }

  public BillingPeriod GetBillingPeriod(string subscriptionId)
  {
    var subscription = GetSubscription(subscriptionId);

    var billingPeriod = GetSubscriptionBillingInterval(subscription);

    return billingPeriod;
  }

  private BillingPeriod GetSubscriptionBillingInterval(Subscription subscription)
  {
    var item = subscription.Items.Data[0];
    var period = item.Price.Recurring.Interval;

    if (period == "month")
    {
      return BillingPeriod.Month;
    }
    if (period == "year")
    {
      return BillingPeriod.Year;
    }
    throw new InvalidBillingPeriodException();
  }

  private DateTime GetEndDate(Subscription subscription)
  {
    DateTime endDate = subscription.CurrentPeriodEnd;

    return endDate;
  }

  private DateTime GetStartDate(Subscription subscription)
  {
    DateTime startDate = subscription.CurrentPeriodStart;

    return startDate;
  }

  private Subscription GetSubscription(string subscriptionId)
  {
    Guard.Against.NullOrEmpty(subscriptionId, nameof(subscriptionId));
    var subscription = _subscriptionService.Get(subscriptionId);

    return subscription;
  }

  public string GetAssociatedProductName(string subscriptionId)
  {
    var subscription = _subscriptionService.Get(subscriptionId); // direct call to Stripe

    var item = subscription.Items.Data[0];
    var productNickname = item.Price.Nickname; // what if this is null?

    if (String.IsNullOrEmpty(productNickname))
    {
      _logger.LogWarning($"Product Nickname from Stripe for subscription Id {subscriptionId} was null or empty!");
      productNickname = "Default";
    }

    return productNickname;
  }
}
