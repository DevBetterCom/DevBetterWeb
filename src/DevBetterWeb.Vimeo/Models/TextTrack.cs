using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Models;

public class TextTrack
{
  [JsonPropertyName("active")]
  public bool Active { get; set; }

  [JsonPropertyName("hls_link")]
  public string HlsLink { get; set; }

  [JsonPropertyName("hls_link_expires_time")]
  public string HlsLinkExpiresTime { get; set; }

  [JsonPropertyName("id")]
  public long Id { get; set; }

  [JsonPropertyName("language")]
  public string Language { get; set; }

  [JsonPropertyName("link")]
  public string Link { get; set; }

  [JsonPropertyName("link_expires_time")]
  public string LinkExpiresTime { get; set; }

  [JsonPropertyName("name")]
  public string Name { get; set; }

  [JsonPropertyName("type")]
  public string Type { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }
}
