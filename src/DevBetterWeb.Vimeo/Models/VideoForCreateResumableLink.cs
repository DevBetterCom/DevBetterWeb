using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Models;

public class VideoForCreateResumableLink
{

	[JsonPropertyName("upload")]
  public Upload Upload { get; set; }
}
