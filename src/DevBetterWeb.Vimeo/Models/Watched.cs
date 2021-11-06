using System.Collections.Generic; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Watched
{
  [JsonPropertyName("added")]
  public bool Added { get; set; }

  [JsonPropertyName("added_time")]
  public string AddedTime { get; set; }

  [JsonPropertyName("options")]
  public List<string> Options { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }
}
