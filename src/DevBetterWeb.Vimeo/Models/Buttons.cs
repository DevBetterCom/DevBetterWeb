using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models
{

  public class Buttons
  {
    [JsonPropertyName("embed")]
    public bool Embed { get; set; }

    [JsonPropertyName("fullscreen")]
    public bool Fullscreen { get; set; }

    [JsonPropertyName("hd")]
    public bool Hd { get; set; }

    [JsonPropertyName("like")]
    public bool Like { get; set; }

    [JsonPropertyName("scaling")]
    public bool Scaling { get; set; }

    [JsonPropertyName("share")]
    public bool Share { get; set; }

    [JsonPropertyName("watchlater")]
    public bool Watchlater { get; set; }
  }

}
