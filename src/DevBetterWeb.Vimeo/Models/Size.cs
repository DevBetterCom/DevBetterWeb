using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Size
{
  [JsonPropertyName("height")]
  public int Height { get; set; }

  [JsonPropertyName("link")]
  public string Link { get; set; }

  [JsonPropertyName("link_with_play_button")]
  public string LinkWithPlayButton { get; set; }

  [JsonPropertyName("width")]
  public int Width { get; set; }
}
