using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Custom
{
  [JsonPropertyName("active")]
  public bool Active { get; set; }

  [JsonPropertyName("link")]
  public string Link { get; set; }

  [JsonPropertyName("sticky")]
  public bool Sticky { get; set; }

  [JsonPropertyName("url")]
  public string Url { get; set; }
}
