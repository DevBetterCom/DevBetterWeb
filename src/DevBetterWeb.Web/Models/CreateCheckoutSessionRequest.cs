using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DevBetterWeb.Web.Models;

public class CreateCheckoutSessionRequest
{
  [JsonProperty("priceId")]
  public string? PriceId { get; set; }
}

public class SetupResponse
{
  [JsonProperty("publishableKey")]
  public string? PublishableKey { get; set; }

  [JsonProperty("proPrice")]
  public string? ProPrice { get; set; }

  [JsonProperty("basicPrice")]
  public string? BasicPrice { get; set; }
}
