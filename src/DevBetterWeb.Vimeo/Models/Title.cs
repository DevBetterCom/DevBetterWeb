using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Title
{
  [JsonPropertyName("name")]
  public string Name { get; set; }

  [JsonPropertyName("owner")]
  public string Owner { get; set; }

  [JsonPropertyName("portrait")]
  public string Portrait { get; set; }
}
