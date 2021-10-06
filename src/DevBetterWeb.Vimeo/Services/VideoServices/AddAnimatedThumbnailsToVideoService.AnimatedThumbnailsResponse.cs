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
    public string AnimatedThumbnailUri
    {
      get
      {
        var linkWithPlay = Thumbnails?.FirstOrDefault(x => !string.IsNullOrEmpty(x.LinkWithPlayButton))?.LinkWithPlayButton;
        if (!string.IsNullOrEmpty(linkWithPlay))
        {
          return linkWithPlay;
        }

        var linkWithoutPlay = Thumbnails?.FirstOrDefault(x => !string.IsNullOrEmpty(x.Link))?.Link;
        return linkWithoutPlay;
      }
    }
    
    [JsonPropertyName("sizes")]
    public List<Thumbnail> Thumbnails { get; set; }
  }
}
