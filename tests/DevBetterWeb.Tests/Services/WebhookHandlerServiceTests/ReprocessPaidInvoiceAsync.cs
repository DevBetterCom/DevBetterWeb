using System;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.ValueObjects;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Infrastructure.PaymentHandler;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.Tests.Services.WebhookHandlerServiceTests;

public class ReprocessPaidInvoiceAsync
{
  private const string InvoiceId = "in_123";
  private const string SubscriptionId = "sub_123";
  private const string CustomerId = "cus_123";
  private const string Email = "new.member@example.com";

  private readonly IPaymentHandlerSubscription _paymentHandlerSubscription = Substitute.For<IPaymentHandlerSubscription>();
  private readonly IPaymentHandlerCustomerService _paymentHandlerCustomerService = Substitute.For<IPaymentHandlerCustomerService>();
  private readonly IPaymentHandlerInvoice _paymentHandlerInvoice = Substitute.For<IPaymentHandlerInvoice>();
  private readonly INewMemberService _newMemberService = Substitute.For<INewMemberService>();
  private readonly IMemberSubscriptionRenewalService _memberSubscriptionRenewalService = Substitute.For<IMemberSubscriptionRenewalService>();
  private readonly IMemberAddBillingActivityService _memberAddBillingActivityService = Substitute.For<IMemberAddBillingActivityService>();
  private readonly IUserLookupService _userLookupService = Substitute.For<IUserLookupService>();
  private readonly WebhookHandlerService _service;

  public ReprocessPaidInvoiceAsync()
  {
    _paymentHandlerSubscription.GetCustomerId(SubscriptionId).Returns(CustomerId);
    _paymentHandlerSubscription.GetStatus(SubscriptionId).Returns("active");
    _paymentHandlerCustomerService.GetCustomer(CustomerId).Returns(new PaymentHandlerCustomer(CustomerId, Email));

    var webhook = new AdminUpdatesWebhook(Substitute.For<IDiscordWebhookService>(),
      Options.Create(new DiscordWebhookUrls { AdminUpdates = "https://example.com/admin-updates" }));

    _service = new WebhookHandlerService(_paymentHandlerSubscription,
      _paymentHandlerCustomerService,
      _paymentHandlerInvoice,
      Substitute.For<IPaymentHandlerEventService>(),
      _newMemberService,
      _memberAddBillingActivityService,
      _memberSubscriptionRenewalService,
      Substitute.For<IMemberCancellationService>(),
      Substitute.For<IMemberSubscriptionEndedAdminEmailService>(),
      _userLookupService,
      Substitute.For<IRepository<Member>>(),
      webhook,
      Substitute.For<IAppLogger<WebhookHandlerService>>());
  }

  private void GivenInvoice(string billingReason, string status = "paid", string subscriptionId = SubscriptionId)
  {
    _paymentHandlerInvoice.GetInvoiceDetails(InvoiceId)
      .Returns(new PaidInvoiceDetails(InvoiceId, subscriptionId, billingReason, status, 20000));
  }

  [Fact]
  public async Task CreatesInvitationAndSendsRegistrationEmailGivenNewSubscriptionInvoice()
  {
    GivenInvoice(StripeConstants.INVOICE_PAYMENT_SUCCEEDED_FOR_SUBSCRIPTION_CREATION);
    var invite = new Invitation(Email, "invite-code", SubscriptionId);
    _newMemberService.CreateInvitationAsync(Email, SubscriptionId).Returns(invite);

    var result = await _service.ReprocessPaidInvoiceAsync(InvoiceId);

    await _newMemberService.Received(1).CreateInvitationAsync(Email, SubscriptionId);
    await _newMemberService.Received(1).SendRegistrationEmailAsync(invite);
    Assert.Contains("new subscription", result);
  }

  [Fact]
  public async Task ExtendsMemberSubscriptionGivenRenewalInvoice()
  {
    GivenInvoice(StripeConstants.INVOICE_PAYMENT_SUCCEEDED_FOR_SUBSCRIPTION_RENEWAL);
    var endDate = new DateTime(2026, 10, 20);
    _paymentHandlerSubscription.GetEndDate(SubscriptionId).Returns(endDate);
    _paymentHandlerSubscription.GetAssociatedProductName(SubscriptionId).Returns("Monthly Plan");

    var result = await _service.ReprocessPaidInvoiceAsync(InvoiceId);

    await _memberSubscriptionRenewalService.Received(1).ExtendMemberSubscription(Email, endDate);
    await _memberAddBillingActivityService.Received(1)
      .AddMemberSubscriptionRenewalBillingActivity(Email, 20000, "Monthly Plan", Arg.Any<Core.Enums.BillingPeriod>());
    await _newMemberService.DidNotReceiveWithAnyArgs().CreateInvitationAsync(default!, default!);
    Assert.Contains("renewal", result);
  }

  [Fact]
  public async Task DoesNothingGivenNewSubscriptionThatIsNotActive()
  {
    GivenInvoice(StripeConstants.INVOICE_PAYMENT_SUCCEEDED_FOR_SUBSCRIPTION_CREATION);
    _paymentHandlerSubscription.GetStatus(SubscriptionId).Returns("canceled");

    var result = await _service.ReprocessPaidInvoiceAsync(InvoiceId);

    await _newMemberService.DidNotReceiveWithAnyArgs().CreateInvitationAsync(default!, default!);
    Assert.Contains("not processed", result);
  }

  [Fact]
  public async Task DoesNothingGivenUnpaidInvoice()
  {
    GivenInvoice(StripeConstants.INVOICE_PAYMENT_SUCCEEDED_FOR_SUBSCRIPTION_CREATION, status: "open");

    var result = await _service.ReprocessPaidInvoiceAsync(InvoiceId);

    await _newMemberService.DidNotReceiveWithAnyArgs().CreateInvitationAsync(default!, default!);
    Assert.Contains("not processed", result);
  }

  [Fact]
  public async Task DoesNothingGivenInvoiceWithoutSubscription()
  {
    GivenInvoice(StripeConstants.INVOICE_PAYMENT_SUCCEEDED_FOR_SUBSCRIPTION_CREATION, subscriptionId: "");

    var result = await _service.ReprocessPaidInvoiceAsync(InvoiceId);

    await _newMemberService.DidNotReceiveWithAnyArgs().CreateInvitationAsync(default!, default!);
    Assert.Contains("not processed", result);
  }

  [Fact]
  public async Task DoesNothingGivenUnhandledBillingReason()
  {
    GivenInvoice("manual");

    var result = await _service.ReprocessPaidInvoiceAsync(InvoiceId);

    await _newMemberService.DidNotReceiveWithAnyArgs().CreateInvitationAsync(default!, default!);
    await _memberSubscriptionRenewalService.DidNotReceiveWithAnyArgs().ExtendMemberSubscription(default!, default);
    Assert.Contains("not handled", result);
  }
}
