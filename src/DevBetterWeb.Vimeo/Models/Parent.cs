using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Parent
{
  [JsonPropertyName("link")]
  public string Link { get; set; }

  [JsonPropertyName("name")]
  public string Name { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }
}
