using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace DevBetterWeb.Vimeo.Models;

public class Embed
{
  [JsonPropertyName("buttons")]
  public Buttons Buttons { get; set; }

  [JsonPropertyName("color")]
  public string Color { get; set; }

  [JsonPropertyName("end_screen")]
  public List<EndScreen> EndScreen { get; set; }

  [JsonPropertyName("event_schedule")]
  public bool EventSchedule { get; set; }

  [JsonPropertyName("logos")]
  public Logos Logos { get; set; }

  [JsonPropertyName("playbar")]
  public bool Playbar { get; set; }

  [JsonPropertyName("speed")]
  public bool Speed { get; set; }

  [JsonPropertyName("title")]
  public Title Title { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }

  [JsonPropertyName("volume")]
  public bool Volume { get; set; }
}
