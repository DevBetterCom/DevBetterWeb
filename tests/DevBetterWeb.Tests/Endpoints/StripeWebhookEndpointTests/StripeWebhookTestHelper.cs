using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Tests.Endpoints.StripeWebhookEndpointTests;

public static class StripeWebhookTestHelper
{
  public const string TestSecret = "whsec_test_secret";

  // Matches the API version pinned by the referenced Stripe.net SDK; events with a
  // different major version are rejected by EventUtility.ConstructEvent.
  public const string CompatibleApiVersion = "2026-07-29.dahlia";

  public static string BuildEventJson(string eventType, string apiVersion = CompatibleApiVersion,
    string dataObjectJson = "{\"object\":\"invoice\",\"billing_reason\":\"manual\"}")
  {
    return "{\"id\":\"evt_test\",\"object\":\"event\",\"api_version\":\"" + apiVersion +
      "\",\"type\":\"" + eventType + "\",\"data\":{\"object\":" + dataObjectJson + "}}";
  }

  public static string SignPayload(string payload, string secret = TestSecret)
  {
    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{timestamp}.{payload}"));
    var signature = Convert.ToHexString(hash).ToLowerInvariant();
    return $"t={timestamp},v1={signature}";
  }

  public static void SetRequest(ControllerBase endpoint, string body, string? signatureHeader)
  {
    var httpContext = new DefaultHttpContext();
    httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
    if (signatureHeader is not null)
    {
      httpContext.Request.Headers["Stripe-Signature"] = signatureHeader;
    }

    endpoint.ControllerContext = new ControllerContext { HttpContext = httpContext };
  }
}
