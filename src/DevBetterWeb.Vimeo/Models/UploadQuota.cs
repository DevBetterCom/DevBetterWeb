using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class UploadQuota
{
  [JsonPropertyName("lifetime")]
  public Lifetime Lifetime { get; set; }

  [JsonPropertyName("periodic")]
  public Periodic Periodic { get; set; }

  [JsonPropertyName("space")]
  public Space Space { get; set; }
}
