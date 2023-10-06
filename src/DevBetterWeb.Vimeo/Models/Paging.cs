using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Models;

public class Paging
{
  [JsonPropertyName("next")]
  public object Next { get; set; }

  [JsonPropertyName("previous")]
  public object Previous { get; set; }

  [JsonPropertyName("first")]
  public string First { get; set; }

  [JsonPropertyName("last")]
  public string Last { get; set; }
}
