using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Infrastructure.Services
{
  public class WebhookHandlerService : IWebhookHandlerService
  {
    private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;
    private readonly IPaymentHandlerCustomer _paymentHandlerCustomer;
    private readonly IPaymentHandlerInvoice _paymentHandlerInvoice;
    private readonly IPaymentHandlerEvent _paymentHandlerEvent;

    private readonly INewMemberService _newMemberService;
    private readonly IMemberAddBillingActivityService _memberAddBillingActivityService;
    private readonly IMemberSubscriptionRenewalService _memberSubscriptionRenewalService;
    private readonly IMemberCancellationService _memberCancellationService;

    private readonly AdminUpdatesWebhook _webhook;

    public WebhookHandlerService(IPaymentHandlerSubscription paymentHandlerSubscription,
      IPaymentHandlerCustomer paymentHandlerCustomer,
      IPaymentHandlerInvoice paymentHandlerInvoice,
      IPaymentHandlerEvent paymentHandlerEvent,
      INewMemberService newMemberService,
      IMemberAddBillingActivityService memberAddBillingActivityService,
      IMemberSubscriptionRenewalService memberSubscriptionRenewalService,
      IMemberCancellationService memberCancellationService,
      AdminUpdatesWebhook webhook)
    {
      _paymentHandlerSubscription = paymentHandlerSubscription;
      _paymentHandlerCustomer = paymentHandlerCustomer;
      _paymentHandlerInvoice = paymentHandlerInvoice;
      _paymentHandlerEvent = paymentHandlerEvent;
      _newMemberService = newMemberService;
      _memberAddBillingActivityService = memberAddBillingActivityService;
      _memberSubscriptionRenewalService = memberSubscriptionRenewalService;
      _memberCancellationService = memberCancellationService;
      _webhook = webhook;
    }

    public async Task HandleCustomerSubscriptionCancelledAtPeriodEndAsync(string json)
    {
      var subscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
      var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
      var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

      await _memberCancellationService.SendFutureCancellationEmailAsync(email);
      var subscriptionPlanName = _paymentHandlerSubscription.GetAssociatedProductName(subscriptionId);
      var billingPeriod = _paymentHandlerSubscription.GetBillingPeriod(subscriptionId);
      await _memberAddBillingActivityService.AddSubscriptionCancellationBillingActivity(email, subscriptionPlanName, billingPeriod);
    }

    public async Task HandleCustomerSubscriptionEndedAsync(string json)
    {
      var subscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
      var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
      var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

      await _memberCancellationService.RemoveUserFromMemberRoleAsync(email);
      await _memberCancellationService.SendCancellationEmailAsync(email);
      var subscriptionPlanName = _paymentHandlerSubscription.GetAssociatedProductName(subscriptionId);
      var billingPeriod = _paymentHandlerSubscription.GetBillingPeriod(subscriptionId);
      await _memberAddBillingActivityService.AddSubscriptionEndingBillingActivity(email, subscriptionPlanName, billingPeriod);
    }

    public async Task HandleCustomerSubscriptionRenewedAsync(string json)
    {
      var subscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
      var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
      var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

      var subscriptionEndDate = _paymentHandlerSubscription.GetEndDate(subscriptionId);

      await _memberSubscriptionRenewalService.ExtendMemberSubscription(email, subscriptionEndDate);

      var paymentAmount = _paymentHandlerInvoice.GetPaymentAmount(json);

      var subscriptionPlanName = _paymentHandlerSubscription.GetAssociatedProductName(subscriptionId);
      var billingPeriod = _paymentHandlerSubscription.GetBillingPeriod(subscriptionId);
      await _memberAddBillingActivityService.AddSubscriptionRenewalBillingActivity(email, paymentAmount, subscriptionPlanName, billingPeriod);
    }

    public async Task HandleNewCustomerSubscriptionAsync(string json)
    {
      var subscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
      var paymentAmount = _paymentHandlerInvoice.GetPaymentAmount(json);

      var status = _paymentHandlerSubscription.GetStatus(subscriptionId);

      if (status == "active")
      {
        var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
        var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

        if (string.IsNullOrEmpty(email))
        {
          throw new InvalidEmailException();
        }

        Invitation invite = await _newMemberService.CreateInvitationAsync(email, subscriptionId);

        var webhookMessage = $"A new customer with email {email} has subscribed to DevBetter. They will be receiving a registration email.";
        await _webhook.Send($"Webhook:\n{webhookMessage}");

        await _newMemberService.SendRegistrationEmailAsync(invite);

        var subscriptionPlanName = _paymentHandlerSubscription.GetAssociatedProductName(subscriptionId);
        var billingPeriod = _paymentHandlerSubscription.GetBillingPeriod(subscriptionId);
        await _memberAddBillingActivityService.AddSubscriptionCreationBillingActivity(email, paymentAmount, subscriptionPlanName, billingPeriod);
      }
    }
  }
}
