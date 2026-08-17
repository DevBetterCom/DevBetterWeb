using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using DevBetterWeb.Web.Endpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSubstitute;
using Stripe;
using Xunit;

namespace DevBetterWeb.Tests.Endpoints.StripeWebhookEndpointTests;

public class PaymentIntentSucceededWebHookTests
{
  private static PaymentIntentSucceededWebHook CreateEndpoint()
  {
    var options = Options.Create(new StripeOptions
    {
      StripePaymentIntentSucceededWebHookSecretKey = StripeWebhookTestHelper.TestSecret
    });

    return new PaymentIntentSucceededWebHook(
      Substitute.For<IAppLogger<InvoicePaidWebHook>>(),
      options);
  }

  [Fact]
  public async Task ReturnsBadRequestGivenMissingSignatureHeader()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.PaymentIntentSucceeded,
      dataObjectJson: "{\"object\":\"payment_intent\",\"id\":\"pi_test\"}");
    StripeWebhookTestHelper.SetRequest(endpoint, json, signatureHeader: null);

    var result = await endpoint.HandleAsync();

    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task ReturnsOkGivenUnexpectedEventType()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.PaymentIntentCreated,
      dataObjectJson: "{\"object\":\"payment_intent\",\"id\":\"pi_test\"}");
    StripeWebhookTestHelper.SetRequest(endpoint, json, StripeWebhookTestHelper.SignPayload(json));

    var result = await endpoint.HandleAsync();

    Assert.IsType<OkResult>(result);
  }

  [Fact]
  public async Task ReturnsOkGivenPaymentIntentSucceededEvent()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.PaymentIntentSucceeded,
      dataObjectJson: "{\"object\":\"payment_intent\",\"id\":\"pi_test\"}");
    StripeWebhookTestHelper.SetRequest(endpoint, json, StripeWebhookTestHelper.SignPayload(json));

    var result = await endpoint.HandleAsync();

    Assert.IsType<OkResult>(result);
  }
}
