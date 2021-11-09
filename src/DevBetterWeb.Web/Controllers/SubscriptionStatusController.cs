using System;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace DevBetterWeb.Web.Controllers;

[Route("get-subscription-status")]
[ApiController]
public class SubscriptionStatusController : Controller
{
  private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;

  public SubscriptionStatusController(IPaymentHandlerSubscription paymentHandlerSubscription)
  {
    _paymentHandlerSubscription = paymentHandlerSubscription;
  }

  [HttpPost]
  public ActionResult AttemptToRetrieveSubscriptionstatus(SubscriptionStatusRequest request)
  {
    try
    {
      var status = _paymentHandlerSubscription.GetStatus(request.SubscriptionId!);

      return Json(new { _status = status });
    }
    catch (Exception e)
    {
      return Json(new { _error = e.Message });
    }
  }

  public class SubscriptionStatusRequest
  {
    [JsonProperty("subscriptionId")]
    public string? SubscriptionId { get; set; }
  }
}
