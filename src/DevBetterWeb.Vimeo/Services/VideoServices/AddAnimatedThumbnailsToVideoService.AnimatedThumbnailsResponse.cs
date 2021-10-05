using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using DevBetterWeb.Vimeo.Models;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class AnimatedThumbnailsResponse
  {
    [JsonPropertyName("clip_uri")]
    public string ClipUri { get; set; }
    [JsonPropertyName("created_on")]
    public long CreatedOn { get; set; }
    public string Status { get; set; }
    public string Uri { get; set; }
    public string PictureId => Uri?.Split("/")?.LastOrDefault();
    [JsonPropertyName("sizes")]
    public List<Thumbnail> Thumbnails { get; set; }
  }
}
