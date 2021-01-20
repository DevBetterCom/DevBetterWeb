using Newtonsoft.Json;

namespace DevBetterWeb.Web.Models
{
  public class ErrorMessage
  {
    [JsonProperty("message")]
    public string? Message { get; set; }
  }
}
