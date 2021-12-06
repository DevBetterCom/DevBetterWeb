using System.Collections.Generic; 
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class ConnectedApps
{
  [JsonPropertyName("options")]
  public List<string> Options { get; set; }

  [JsonPropertyName("total")]
  public int Total { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }

  [JsonPropertyName("all_scopes")]
  public List<object> AllScopes { get; set; }

  [JsonPropertyName("is_connected")]
  public bool IsConnected { get; set; }

  [JsonPropertyName("needed_scopes")]
  public List<object> NeededScopes { get; set; }
}
