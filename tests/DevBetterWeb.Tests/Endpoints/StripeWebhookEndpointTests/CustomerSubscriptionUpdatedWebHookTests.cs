using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.Endpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSubstitute;
using Stripe;
using Xunit;

namespace DevBetterWeb.Tests.Endpoints.StripeWebhookEndpointTests;

public class CustomerSubscriptionUpdatedWebHookTests
{
  private readonly IPaymentHandlerEventService _paymentHandlerEventService = Substitute.For<IPaymentHandlerEventService>();
  private readonly IPaymentHandlerSubscription _paymentHandlerSubscription = Substitute.For<IPaymentHandlerSubscription>();
  private readonly IWebhookHandlerService _webhookHandlerService = Substitute.For<IWebhookHandlerService>();

  private CustomerSubscriptionUpdatedWebHook CreateEndpoint()
  {
    var options = Options.Create(new StripeOptions
    {
      StripeCustomerSubscriptionUpdatedWebHookSecretKey = StripeWebhookTestHelper.TestSecret
    });

    return new CustomerSubscriptionUpdatedWebHook(
      Substitute.For<IAppLogger<CustomerSubscriptionDeletedWebHook>>(),
      options,
      _paymentHandlerEventService,
      _paymentHandlerSubscription,
      _webhookHandlerService);
  }

  [Fact]
  public async Task ReturnsBadRequestGivenMissingSignatureHeader()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.CustomerSubscriptionUpdated);
    StripeWebhookTestHelper.SetRequest(endpoint, json, signatureHeader: null);

    var result = await endpoint.HandleAsync();

    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task ReturnsOkWithoutProcessingGivenUnexpectedEventType()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.CustomerSubscriptionCreated);
    StripeWebhookTestHelper.SetRequest(endpoint, json, StripeWebhookTestHelper.SignPayload(json));

    var result = await endpoint.HandleAsync();

    Assert.IsType<OkResult>(result);
    _paymentHandlerEventService.DidNotReceiveWithAnyArgs().FromJson(default!);
  }
}
