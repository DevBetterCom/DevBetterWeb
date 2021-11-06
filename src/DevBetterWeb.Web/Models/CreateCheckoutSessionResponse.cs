using Newtonsoft.Json;

namespace DevBetterWeb.Web.Models;

public class CreateCheckoutSessionResponse
{
  [JsonProperty("sessionId")]
  public string? SessionId { get; set; }
}
