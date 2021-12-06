using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Skill
{
  [JsonPropertyName("name")]
  public string Name { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }
}
