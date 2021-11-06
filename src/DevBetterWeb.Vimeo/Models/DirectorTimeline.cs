using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class DirectorTimeline
{
  [JsonPropertyName("pitch")]
  public int Pitch { get; set; }

  [JsonPropertyName("roll")]
  public int Roll { get; set; }

  [JsonPropertyName("time_code")]
  public int TimeCode { get; set; }

  [JsonPropertyName("yaw")]
  public int Yaw { get; set; }
}
