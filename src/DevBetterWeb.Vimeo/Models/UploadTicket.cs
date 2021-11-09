using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Models;

public class UploadTicket
{
  [JsonPropertyName("ticket_id")]
  public string TicketId { get; set; }

  [JsonPropertyName("uri")]
  public string Uri { get; set; }

  [JsonPropertyName("complete_uri")]
  public string CompleteUri { get; set; }

  [JsonPropertyName("upload_uri")]
  public string UploadUri { get; set; }

  [JsonPropertyName("upload_uri_secure")]
  public string UploadUriSecure { get; set; }

  [JsonPropertyName("upload_link")]
  public string UploadLink { get; set; }

  [JsonPropertyName("upload_link_secure")]
  public string UploadLinkSecure { get; set; }

  [JsonPropertyName("type")]
  public string Type { get; set; }

  [JsonPropertyName("user")]
  public User User { get; set; }

  [JsonPropertyName("video")]
  public Video Video { get; set; }

  [JsonPropertyName("upload")]
  public Upload Upload { get; set; }

  [JsonPropertyName("transcode")]
  public Transcode Transcode { get; set; }

  [JsonPropertyName("quota")]
  public UploadQuota UploadQuota { get; set; }
}
