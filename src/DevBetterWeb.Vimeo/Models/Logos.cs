using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Logos
{
  [JsonPropertyName("custom")]
  public Custom Custom { get; set; }

  [JsonPropertyName("vimeo")]
  public bool Vimeo { get; set; }
}
