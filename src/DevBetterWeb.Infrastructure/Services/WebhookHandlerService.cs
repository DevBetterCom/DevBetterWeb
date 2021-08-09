using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.DiscordWebooks;

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

    private readonly IMemberSubscriptionEndedAdminEmailService _memberSubscriptionEndedAdminEmailService;

    private readonly IUserLookupService _userLookupService;
    private readonly IRepository<Member> _repository;

    private readonly AdminUpdatesWebhook _webhook;

    // TODO: This is a lot of injected dependencies...
    public WebhookHandlerService(IPaymentHandlerSubscription paymentHandlerSubscription,
      IPaymentHandlerCustomer paymentHandlerCustomer,
      IPaymentHandlerInvoice paymentHandlerInvoice,
      IPaymentHandlerEvent paymentHandlerEvent,
      INewMemberService newMemberService,
      IMemberAddBillingActivityService memberAddBillingActivityService,
      IMemberSubscriptionRenewalService memberSubscriptionRenewalService,
      IMemberCancellationService memberCancellationService,
      IMemberSubscriptionEndedAdminEmailService memberSubscriptionEndedAdminEmailService,
      IUserLookupService userLookupService,
      IRepository<Member> repository,
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
      _memberSubscriptionEndedAdminEmailService = memberSubscriptionEndedAdminEmailService;
      _userLookupService = userLookupService;
      _repository = repository;
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
      await _memberAddBillingActivityService.AddMemberSubscriptionCancellationBillingActivity(email, subscriptionPlanName, billingPeriod);
    }

    public async Task HandleCustomerSubscriptionEndedAsync(string json)
    {
      var subscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
      var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
      var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

      await _memberCancellationService.RemoveUserFromMemberRoleAsync(email);
      await _memberCancellationService.SendCancellationEmailAsync(email);
      await _memberSubscriptionEndedAdminEmailService.SendMemberSubscriptionEndedEmailAsync(email);
      var subscriptionPlanName = _paymentHandlerSubscription.GetAssociatedProductName(subscriptionId);
      var billingPeriod = _paymentHandlerSubscription.GetBillingPeriod(subscriptionId);
      await _memberAddBillingActivityService.AddMemberSubscriptionEndingBillingActivity(email, subscriptionPlanName, billingPeriod);
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
      await _memberAddBillingActivityService.AddMemberSubscriptionRenewalBillingActivity(email, paymentAmount, subscriptionPlanName, billingPeriod);
    }

    public async Task HandleNewCustomerSubscriptionAsync(string json)
    {
      var paymentHandlerSubscriptionId = _paymentHandlerEvent.GetSubscriptionId(json);
      var paymentAmount = _paymentHandlerInvoice.GetPaymentAmount(json);

      var newSubscriberIsAlreadyMember = await NewCustomerSubscriptionWithEmailAlreadyMember(paymentHandlerSubscriptionId);

      if (newSubscriberIsAlreadyMember)
      {
        await HandleNewCustomerSubscriptionWithEmailAlreadyMember(paymentHandlerSubscriptionId, paymentAmount);
      }
      else
      {

        var status = _paymentHandlerSubscription.GetStatus(paymentHandlerSubscriptionId);

        if (status == "active")
        {
          var customerId = _paymentHandlerSubscription.GetCustomerId(paymentHandlerSubscriptionId);
          var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

          if (string.IsNullOrEmpty(email))
          {
            throw new InvalidEmailException();
          }

          Invitation invite = await _newMemberService.CreateInvitationAsync(email, paymentHandlerSubscriptionId);

          var webhookMessage = $"A new customer with email {email} has subscribed to DevBetter. They will be receiving a registration email.";
          await _webhook.Send($"Webhook:\n{webhookMessage}");

          await _newMemberService.SendRegistrationEmailAsync(invite);

          await AddNewSubscriberBillingActivity(paymentHandlerSubscriptionId, email, paymentAmount);
        }
      }
    }

    private async Task AddNewSubscriberBillingActivity(string subscriptionId, string email, decimal paymentAmount)
    {
      var subscriptionPlanName = _paymentHandlerSubscription.GetAssociatedProductName(subscriptionId);
      var billingPeriod = _paymentHandlerSubscription.GetBillingPeriod(subscriptionId);
      await _memberAddBillingActivityService.AddMemberSubscriptionCreationBillingActivity(email, paymentAmount, subscriptionPlanName, billingPeriod);
    }

    // Below two methods are for handling migration from Launchpass to Stripe
    private async Task<bool> NewCustomerSubscriptionWithEmailAlreadyMember(string subscriptionId)
    {
      var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
      var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

      var userIsMember = await _userLookupService.FindUserIsMemberByEmailAsync(email);

      return userIsMember;

    }

    private async Task HandleNewCustomerSubscriptionWithEmailAlreadyMember(string subscriptionId, decimal paymentAmount)
    {
      var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
      var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

      await AddNewSubscriberBillingActivity(subscriptionId, email, paymentAmount);

      var subscriptionDateTimeRange = _paymentHandlerSubscription.GetDateTimeRange(subscriptionId);

      var userId = await _userLookupService.FindUserIdByEmailAsync(email);
      var spec = new MemberByUserIdSpec(userId);
      var member = await _repository.GetBySpecAsync(spec);

      if(member != null)
      {
        member.AddSubscription(subscriptionDateTimeRange);
      }
    }
  }
}
