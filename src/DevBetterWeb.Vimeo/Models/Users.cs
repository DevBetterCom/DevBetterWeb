using System.Collections.Generic; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Users
{
  [JsonPropertyName("options")]
  public List<string> Options { get; set; }

  [JsonPropertyName("total")]
  public int Total { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }
}
