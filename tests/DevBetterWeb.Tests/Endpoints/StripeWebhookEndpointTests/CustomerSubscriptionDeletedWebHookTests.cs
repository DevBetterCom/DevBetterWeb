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

public class CustomerSubscriptionDeletedWebHookTests
{
  private readonly IWebhookHandlerService _webhookHandlerService = Substitute.For<IWebhookHandlerService>();

  private CustomerSubscriptionDeletedWebHook CreateEndpoint()
  {
    var options = Options.Create(new StripeOptions
    {
      StripeCustomerSubscriptionDeletedWebHookSecretKey = StripeWebhookTestHelper.TestSecret
    });

    return new CustomerSubscriptionDeletedWebHook(
      Substitute.For<IAppLogger<CustomerSubscriptionDeletedWebHook>>(),
      options,
      _webhookHandlerService);
  }

  [Fact]
  public async Task ReturnsBadRequestGivenMissingSignatureHeader()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.CustomerSubscriptionDeleted);
    StripeWebhookTestHelper.SetRequest(endpoint, json, signatureHeader: null);

    var result = await endpoint.HandleAsync();

    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task ReturnsOkWithoutProcessingGivenUnexpectedEventType()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.CustomerDeleted,
      dataObjectJson: "{\"object\":\"customer\",\"id\":\"cus_test\"}");
    StripeWebhookTestHelper.SetRequest(endpoint, json, StripeWebhookTestHelper.SignPayload(json));

    var result = await endpoint.HandleAsync();

    Assert.IsType<OkResult>(result);
    await _webhookHandlerService.DidNotReceiveWithAnyArgs().HandleCustomerSubscriptionEndedAsync(default!);
  }

  [Fact]
  public async Task HandlesSubscriptionEndedGivenCustomerSubscriptionDeletedEvent()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.CustomerSubscriptionDeleted,
      dataObjectJson: "{\"object\":\"subscription\",\"id\":\"sub_test\"}");
    StripeWebhookTestHelper.SetRequest(endpoint, json, StripeWebhookTestHelper.SignPayload(json));

    var result = await endpoint.HandleAsync();

    Assert.IsType<OkResult>(result);
    await _webhookHandlerService.Received(1).HandleCustomerSubscriptionEndedAsync(json);
  }
}
