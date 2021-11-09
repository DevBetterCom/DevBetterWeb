using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class AddAnimatedThumbnailsToVideoRequest
{
  [JsonPropertyName("start_time")]
  public int StartTime { get; set; }

  [JsonPropertyName("duration")]
  public int Duration { get; set; }

  [JsonIgnore]
  public long? VideoId { get; set; }

  public AddAnimatedThumbnailsToVideoRequest(long videoId, int startTime, int duration)
  {
    VideoId = videoId;
    StartTime = startTime;
    Duration = duration;
  }
}
