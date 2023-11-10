using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Infrastructure.Interfaces;

namespace DevBetterWeb.Infrastructure.Services;

public class WebhookHandlerService : IWebhookHandlerService
{
  private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;
  private readonly IPaymentHandlerCustomerService _paymentHandlerCustomerService;
  private readonly IPaymentHandlerInvoice _paymentHandlerInvoice;
  private readonly IPaymentHandlerEventService _paymentHandlerEventService;

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
    IPaymentHandlerCustomerService paymentHandlerCustomerService,
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
    _paymentHandlerCustomerService = paymentHandlerCustomerService;
    _paymentHandlerInvoice = paymentHandlerInvoice;
    _paymentHandlerEventService = paymentHandlerEvent;
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
    var paymentHandlerEvent = _paymentHandlerEventService.FromJson(json);
    var customerId = _paymentHandlerSubscription.GetCustomerId(paymentHandlerEvent.SubscriptionId);
    var paymentHandlerCustomer = _paymentHandlerCustomerService.GetCustomer(customerId);

    if (await IsAlumniAsync(paymentHandlerCustomer.Email))
    {
	    return;
    }

		await _memberCancellationService.SendFutureCancellationEmailAsync(paymentHandlerCustomer.Email);
    var subscriptionPlanName = _paymentHandlerSubscription.GetAssociatedProductName(paymentHandlerEvent.SubscriptionId);
    var billingPeriod = _paymentHandlerSubscription.GetBillingPeriod(paymentHandlerEvent.SubscriptionId);
    await _memberAddBillingActivityService.AddMemberSubscriptionCancellationBillingActivity(paymentHandlerCustomer.Email, subscriptionPlanName, billingPeriod);
  }

  public async Task HandleCustomerSubscriptionEndedAsync(string json)
  {
		// TODO: Log all JSON from these webhooks to a db table - perhaps use a decorator on this service
    var paymentHandlerEvent = _paymentHandlerEventService.FromJson(json);
    var customerId = _paymentHandlerSubscription.GetCustomerId(paymentHandlerEvent.SubscriptionId);
    var paymentHandlerCustomer = _paymentHandlerCustomerService.GetCustomer(customerId);

		if (await IsAlumniAsync(paymentHandlerCustomer.Email))
		{
			return;
		}

		await _memberCancellationService.RemoveUserFromMemberRoleAsync(paymentHandlerCustomer.Email);
    await _memberCancellationService.SendCancellationEmailAsync(paymentHandlerCustomer.Email);

    var memberByEmailSpec = new MemberByEmailSpec(paymentHandlerCustomer.Email);
    var memberFullInfo = await _repository.FirstOrDefaultAsync(memberByEmailSpec);

		await _memberSubscriptionEndedAdminEmailService.SendMemberSubscriptionEndedEmailAsync(paymentHandlerCustomer.Email, memberFullInfo);
    var subscriptionPlanName = _paymentHandlerSubscription.GetAssociatedProductName(paymentHandlerEvent.SubscriptionId);
    var billingPeriod = _paymentHandlerSubscription.GetBillingPeriod(paymentHandlerEvent.SubscriptionId);
    await _memberAddBillingActivityService.AddMemberSubscriptionEndingBillingActivity(paymentHandlerCustomer.Email, subscriptionPlanName, billingPeriod);
  }

  public async Task HandleCustomerSubscriptionRenewedAsync(string json)
  {
		// TODO: Log all JSON from these webhooks to a db table - perhaps use a decorator on this service
		var paymentHandlerEvent = _paymentHandlerEventService.FromJson(json);
    var customerId = _paymentHandlerSubscription.GetCustomerId(paymentHandlerEvent.SubscriptionId);
    var paymentHandlerCustomer = _paymentHandlerCustomerService.GetCustomer(customerId);

    if (await IsAlumniAsync(paymentHandlerCustomer.Email))
    {
	    return;
    }

    var subscriptionEndDate = _paymentHandlerSubscription.GetEndDate(paymentHandlerEvent.SubscriptionId);

    await _memberSubscriptionRenewalService.ExtendMemberSubscription(paymentHandlerCustomer.Email, subscriptionEndDate);

    var paymentAmount = _paymentHandlerInvoice.GetPaymentAmount(json);

    var subscriptionPlanName = _paymentHandlerSubscription.GetAssociatedProductName(paymentHandlerEvent.SubscriptionId);
    var billingPeriod = _paymentHandlerSubscription.GetBillingPeriod(paymentHandlerEvent.SubscriptionId);
    await _memberAddBillingActivityService.AddMemberSubscriptionRenewalBillingActivity(paymentHandlerCustomer.Email, paymentAmount, subscriptionPlanName, billingPeriod);
  }

  public async Task HandleNewCustomerSubscriptionAsync(string json)
  {
		// TODO: Log all JSON from these webhooks to a db table - perhaps use a decorator on this service
		var paymentHandlerEvent = _paymentHandlerEventService.FromJson(json);
    if (string.IsNullOrEmpty(paymentHandlerEvent.SubscriptionId))
    {
      _logger.LogWarning("Payment handler subscriptionId is null or empty", json);
    }
    var paymentAmount = _paymentHandlerInvoice.GetPaymentAmount(json);

    var newSubscriberIsAlreadyMember = await IsNewCustomerSubscriptionWithEmailAlreadyMember(paymentHandlerEvent.SubscriptionId);

    if (newSubscriberIsAlreadyMember)
    {
      _logger.LogInformation("New subscriber is an existing devBetter member", json);

      await HandleNewCustomerSubscriptionWithEmailAlreadyMember(paymentHandlerEvent.SubscriptionId, paymentAmount);
    }
    else
    {
      var status = _paymentHandlerSubscription.GetStatus(paymentHandlerEvent.SubscriptionId);
      _logger.LogInformation($"Subscription status: {status}");

      if (status == "active")
      {
        var customerId = _paymentHandlerSubscription.GetCustomerId(paymentHandlerEvent.SubscriptionId);
        var paymentHandlerCustomer = _paymentHandlerCustomerService.GetCustomer(customerId);

        if (string.IsNullOrEmpty(paymentHandlerCustomer.Email))
        {
          throw new InvalidEmailException();
        }

        Invitation invite = await _newMemberService.CreateInvitationAsync(paymentHandlerCustomer.Email, paymentHandlerEvent.SubscriptionId);

        var webhookMessage = $"A new customer with email {paymentHandlerCustomer.Email} has subscribed to DevBetter. They will be receiving a registration email.";
        await _webhook.SendAsync($"Webhook:\n{webhookMessage}");

        await _newMemberService.SendRegistrationEmailAsync(invite);

        // moved to NewMemberRegister.cshtml.cs
        //await AddNewSubscriberBillingActivity(paymentHandlerSubscriptionId, email, paymentAmount);
      }
    }
  }

  private async Task<bool> IsAlumniAsync(string email)
  {
	  var isAlumni = await _userLookupService.FindUserIsAlumniByEmailAsync(email);

	  return isAlumni;
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
    var paymentHandlerCustomer = _paymentHandlerCustomerService.GetCustomer(customerId);

    var userIsMember = await _userLookupService.FindUserIsMemberByEmailAsync(paymentHandlerCustomer.Email);

    return userIsMember;
  }

  private async Task HandleNewCustomerSubscriptionWithEmailAlreadyMember(string subscriptionId, decimal paymentAmount)
  {
    var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
    var paymentHandlerCustomer = _paymentHandlerCustomerService.GetCustomer(customerId);

    await AddNewSubscriberBillingActivity(subscriptionId, paymentHandlerCustomer.Email, paymentAmount);

    var subscriptionDateTimeRange = _paymentHandlerSubscription.GetDateTimeRange(subscriptionId);

    var spec = new MemberByEmailSpec(paymentHandlerCustomer.Email);
    var member = await _repository.FirstOrDefaultAsync(spec);

		// TODO this should take in the subscription plan id - currently hard-coded to monthly
		member?.AddSubscription(subscriptionDateTimeRange, 1);
	}
}
