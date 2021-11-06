using System; 
using System.Collections.Generic; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Follow
{
  [JsonPropertyName("added")]
  public bool Added { get; set; }

  [JsonPropertyName("added_time")]
  public DateTime AddedTime { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }

  [JsonPropertyName("options")]
  public List<string> Options { get; set; }
}
