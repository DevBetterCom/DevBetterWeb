using System.Collections.Generic; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Feed
{
  [JsonPropertyName("options")]
  public List<string> Options { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }
}
