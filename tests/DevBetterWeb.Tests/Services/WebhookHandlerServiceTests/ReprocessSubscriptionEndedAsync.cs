using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Infrastructure.PaymentHandler;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.Tests.Services.WebhookHandlerServiceTests;

public class ReprocessSubscriptionEndedAsync
{
  private const string SubscriptionId = "sub_123";
  private const string CustomerId = "cus_123";
  private const string Email = "ending.member@example.com";

  private readonly IPaymentHandlerSubscription _paymentHandlerSubscription = Substitute.For<IPaymentHandlerSubscription>();
  private readonly IPaymentHandlerCustomerService _paymentHandlerCustomerService = Substitute.For<IPaymentHandlerCustomerService>();
  private readonly IMemberCancellationService _memberCancellationService = Substitute.For<IMemberCancellationService>();
  private readonly WebhookHandlerService _service;

  public ReprocessSubscriptionEndedAsync()
  {
    _paymentHandlerSubscription.GetCustomerId(SubscriptionId).Returns(CustomerId);
    _paymentHandlerCustomerService.GetCustomer(CustomerId).Returns(new PaymentHandlerCustomer(CustomerId, Email));

    var webhook = new AdminUpdatesWebhook(Substitute.For<IDiscordWebhookService>(),
      Options.Create(new DiscordWebhookUrls { AdminUpdates = "https://example.com/admin-updates" }));

    _service = new WebhookHandlerService(_paymentHandlerSubscription,
      _paymentHandlerCustomerService,
      Substitute.For<IPaymentHandlerInvoice>(),
      Substitute.For<IPaymentHandlerEventService>(),
      Substitute.For<INewMemberService>(),
      Substitute.For<IMemberAddBillingActivityService>(),
      Substitute.For<IMemberSubscriptionRenewalService>(),
      _memberCancellationService,
      Substitute.For<IMemberSubscriptionEndedAdminEmailService>(),
      Substitute.For<IUserLookupService>(),
      Substitute.For<IRepository<Member>>(),
      webhook,
      Substitute.For<IAppLogger<WebhookHandlerService>>());
  }

  [Fact]
  public async Task RemovesMemberRoleGivenCanceledSubscription()
  {
    _paymentHandlerSubscription.GetStatus(SubscriptionId).Returns("canceled");

    var result = await _service.ReprocessSubscriptionEndedAsync(SubscriptionId);

    await _memberCancellationService.Received(1).RemoveUserFromMemberRoleAsync(Email);
    Assert.Contains("processed as ended", result);
  }

  [Fact]
  public async Task DoesNothingGivenActiveSubscription()
  {
    _paymentHandlerSubscription.GetStatus(SubscriptionId).Returns("active");

    var result = await _service.ReprocessSubscriptionEndedAsync(SubscriptionId);

    await _memberCancellationService.DidNotReceiveWithAnyArgs().RemoveUserFromMemberRoleAsync(default!);
    Assert.Contains("not processed", result);
  }
}
