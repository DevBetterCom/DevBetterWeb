using System.Text.Json.Serialization;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetAnimatedThumbnailRequest
{
  public string PictureId { get; set; }

  public long VideoId { get; set; }

  public GetAnimatedThumbnailRequest(long videoId, string pictureId)
  {
    VideoId = videoId;
    PictureId = pictureId;
  }
}
