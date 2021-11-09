using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Models;

public class ActiveTextTrack
{
  [JsonPropertyName("active")]
  public bool Active { get; set; }

  public ActiveTextTrack(bool active = true)
  {
    Active = active;
  }
}
