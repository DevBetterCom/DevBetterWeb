using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Lifetime
{
  [JsonPropertyName("free")]
  public long? Free { get; set; }

  [JsonPropertyName("max")]
  public long? Max { get; set; }

  [JsonPropertyName("used")]
  public long? Used { get; set; }
}
