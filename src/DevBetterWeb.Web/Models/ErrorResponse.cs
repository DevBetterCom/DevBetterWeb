using Newtonsoft.Json;

namespace DevBetterWeb.Web.Models
{
  public class ErrorResponse
  {
    [JsonProperty("error")]
    public ErrorMessage? ErrorMessage { get; set; }
  }
}
