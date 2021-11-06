using Stripe;
using Xunit;

namespace DevBetterWeb.Tests.Integration.Stripe;

public class EventUtilityParseEvent
{
  private readonly string _jsonFile = "Integration/Stripe/stripeJson1.json";

  [Fact]
  public void ParseJsonToSubscriptionId()
  {
    string json = System.IO.File.ReadAllText(_jsonFile);
    var stripeEvent = EventUtility.ParseEvent(json);

    var invoice = (Invoice)stripeEvent.Data.Object;
    string subscriptionId = invoice.SubscriptionId;
    Assert.Equal("sub_K1hzaxOt9gb2TB", subscriptionId);
  }
}
