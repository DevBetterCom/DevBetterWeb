using DevBetterWeb.Vimeo.Models;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class UpdateVideoDetailsRequest
{
  public long VideoId { get; set; }
  public Video Video { get; set; }

  public UpdateVideoDetailsRequest(long videoId, Video video)
  {
    VideoId = videoId;
    Video = video;
  }
}
