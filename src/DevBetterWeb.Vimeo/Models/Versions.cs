using System.Collections.Generic; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Versions
{
  [JsonPropertyName("current_uri")]
  public string CurrentUri { get; set; }

  [JsonPropertyName("options")]
  public List<string> Options { get; set; }

  [JsonPropertyName("resource_key")]
  public string ResourceKey { get; set; }

  [JsonPropertyName("total")]
  public int Total { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }
}
