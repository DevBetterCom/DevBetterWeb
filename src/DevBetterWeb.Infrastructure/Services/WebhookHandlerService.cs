﻿using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;

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
    private readonly IMemberSubscriptionFactory _memberSubscriptionCreationService;
    private readonly IMemberCancellationService _memberCancellationService;

    private readonly IUserLookupService _userLookupService;
    private readonly IRepository<Member> _repository;

    private readonly AdminUpdatesWebhook _webhook;

    public WebhookHandlerService(IPaymentHandlerSubscription paymentHandlerSubscription,
      IPaymentHandlerCustomer paymentHandlerCustomer,
      IPaymentHandlerInvoice paymentHandlerInvoice,
      IPaymentHandlerEvent paymentHandlerEvent,
      INewMemberService newMemberService,
      IMemberAddBillingActivityService memberAddBillingActivityService,
      IMemberSubscriptionRenewalService memberSubscriptionRenewalService,
      IMemberSubscriptionFactory memberSubscriptionCreationService,
      IMemberCancellationService memberCancellationService,
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
      _memberSubscriptionCreationService = memberSubscriptionCreationService;
      _memberCancellationService = memberCancellationService;
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

      var newSubscriberIsAlreadyMember = await NewCustomerSubscriptionWithEmailAlreadyMember(subscriptionId);

      if (newSubscriberIsAlreadyMember)
      {
        await HandleNewCustomerSubscriptionWithEmailAlreadyMember(subscriptionId, paymentAmount);
      }
      else
      {

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

          await AddNewSubscriberBillingActivity(subscriptionId, email, paymentAmount);
        }
      }
    }

    private async Task AddNewSubscriberBillingActivity(string subscriptionId, string email, decimal paymentAmount)
    {
      var subscriptionPlanName = _paymentHandlerSubscription.GetAssociatedProductName(subscriptionId);
      var billingPeriod = _paymentHandlerSubscription.GetBillingPeriod(subscriptionId);
      await _memberAddBillingActivityService.AddSubscriptionCreationBillingActivity(email, paymentAmount, subscriptionPlanName, billingPeriod);
    }

    // Below two methods are for handling migration from Launchpass to Stripe
    private async Task<bool> NewCustomerSubscriptionWithEmailAlreadyMember(string subscriptionId)
    {
      var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
      var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

      var userIsMember = await _userLookupService.FindUserIsMemberByEmailAsync(email);

      if (userIsMember)
      {
        return true;
      }

      return false;

    }

    private async Task HandleNewCustomerSubscriptionWithEmailAlreadyMember(string subscriptionId, decimal paymentAmount)
    {
      var customerId = _paymentHandlerSubscription.GetCustomerId(subscriptionId);
      var email = _paymentHandlerCustomer.GetCustomerEmail(customerId);

      await AddNewSubscriberBillingActivity(subscriptionId, email, paymentAmount);

      var subscriptionDateTimeRange = _paymentHandlerSubscription.GetDateTimeRange(subscriptionId);

      var userId = await _userLookupService.FindUserIdByEmailAsync(email);
      var spec = new MemberByUserIdSpec(userId);
      var member = _repository.GetBySpecAsync(spec);

      await _memberSubscriptionCreationService.CreateSubscriptionForMemberAsync(member.Id, subscriptionDateTimeRange);
    }
  }
}
