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

public class InvoicePaidWebHookTests
{
  private readonly IWebhookHandlerService _webhookHandlerService = Substitute.For<IWebhookHandlerService>();
  private readonly IPaymentHandlerInvoice _paymentHandlerInvoice = Substitute.For<IPaymentHandlerInvoice>();

  private InvoicePaidWebHook CreateEndpoint()
  {
    var options = Options.Create(new StripeOptions
    {
      StripeInvoicePaidWebHookSecretKey = StripeWebhookTestHelper.TestSecret
    });

    return new InvoicePaidWebHook(
      Substitute.For<IAppLogger<InvoicePaidWebHook>>(),
      options,
      _webhookHandlerService,
      _paymentHandlerInvoice);
  }

  [Fact]
  public async Task ReturnsBadRequestGivenMissingSignatureHeader()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.InvoicePaid);
    StripeWebhookTestHelper.SetRequest(endpoint, json, signatureHeader: null);

    var result = await endpoint.HandleAsync();

    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task ReturnsBadRequestGivenInvalidSignature()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.InvoicePaid);
    StripeWebhookTestHelper.SetRequest(endpoint, json, "t=123,v1=invalid");

    var result = await endpoint.HandleAsync();

    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task ReturnsOkWithoutProcessingGivenUnexpectedEventType()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.InvoiceCreated);
    StripeWebhookTestHelper.SetRequest(endpoint, json, StripeWebhookTestHelper.SignPayload(json));

    var result = await endpoint.HandleAsync();

    Assert.IsType<OkResult>(result);
    await _webhookHandlerService.DidNotReceiveWithAnyArgs().HandleNewCustomerSubscriptionAsync(default!);
  }

  [Fact]
  public async Task HandlesNewSubscriptionGivenInvoicePaidForSubscriptionCreation()
  {
    var endpoint = CreateEndpoint();
    var json = StripeWebhookTestHelper.BuildEventJson(EventTypes.InvoicePaid);
    _paymentHandlerInvoice.GetBillingReason(json).Returns("subscription_create");
    StripeWebhookTestHelper.SetRequest(endpoint, json, StripeWebhookTestHelper.SignPayload(json));

    var result = await endpoint.HandleAsync();

    Assert.IsType<OkResult>(result);
    await _webhookHandlerService.Received(1).HandleNewCustomerSubscriptionAsync(json);
  }
}
