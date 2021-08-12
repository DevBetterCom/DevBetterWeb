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
    private readonly IPaymentHandlerEventService _paymentHandlerEvent;

    private readonly INewMemberService _newMemberService;
    private readonly IMemberAddBillingActivityService _memberAddBillingActivityService;
    private readonly IMemberSubscriptionRenewalService _memberSubscriptionRenewalService;
    private readonly IMemberCancellationService _memberCancellationService;

    private readonly IMemberSubscriptionEndedAdminEmailService _memberSubscriptionEndedAdminEmailService;

    private readonly IUserLookupService _userLookupService;
    private readonly IRepository<Member> _repository;

    private readonly AdminUpdatesWebhook _webhook;
    private readonly IAppLogger<WebhookHandlerService> _logger;

    // TODO: This is a lot of injected dependencies...
    public WebhookHandlerService(IPaymentHandlerSubscription paymentHandlerSubscription,
      IPaymentHandlerCustomer paymentHandlerCustomer,
      IPaymentHandlerInvoice paymentHandlerInvoice,
      IPaymentHandlerEventService paymentHandlerEvent,
      INewMemberService newMemberService,
      IMemberAddBillingActivityService memberAddBillingActivityService,
      IMemberSubscriptionRenewalService memberSubscriptionRenewalService,
      IMemberCancellationService memberCancellationService,
      IMemberSubscriptionEndedAdminEmailService memberSubscriptionEndedAdminEmailService,
      IUserLookupService userLookupService,
      IRepository<Member> repository,
      AdminUpdatesWebhook webhook,
      IAppLogger<WebhookHandlerService> logger)
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
      _logger = logger;
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
      if(string.IsNullOrEmpty(paymentHandlerSubscriptionId))
      {
        _logger.LogWarning("Payment handler subscriptionId is null or empty", json);
      }
      var paymentAmount = _paymentHandlerInvoice.GetPaymentAmount(json);

      var newSubscriberIsAlreadyMember = await IsNewCustomerSubscriptionWithEmailAlreadyMember(paymentHandlerSubscriptionId);

      if (newSubscriberIsAlreadyMember)
      {
        _logger.LogInformation("New subscriber is an existing devBetter member", json);

        await HandleNewCustomerSubscriptionWithEmailAlreadyMember(paymentHandlerSubscriptionId, paymentAmount);
      }
      else
      {
        var status = _paymentHandlerSubscription.GetStatus(paymentHandlerSubscriptionId);
        _logger.LogInformation($"Subscription status: {status}");

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

          // moved to NewMemberRegister.cshtml.cs
          //await AddNewSubscriberBillingActivity(paymentHandlerSubscriptionId, email, paymentAmount);
        }
      }
    }

    private Task AddNewSubscriberBillingActivity(string subscriptionId, string email, decimal paymentAmount)
    {
      var subscriptionPlanName = _paymentHandlerSubscription.GetAssociatedProductName(subscriptionId);
      var billingPeriod = _paymentHandlerSubscription.GetBillingPeriod(subscriptionId);
      return _memberAddBillingActivityService.AddMemberSubscriptionCreationBillingActivity(email, paymentAmount, subscriptionPlanName, billingPeriod);
    }

    // Below two methods are for handling migration from Launchpass to Stripe
    private async Task<bool> IsNewCustomerSubscriptionWithEmailAlreadyMember(string subscriptionId)
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
        // TODO this should take in the subscription plan id
        member.AddSubscription(subscriptionDateTimeRange);
      }
    }
  }
}
