using System.Collections.Generic;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetAllAnimatedThumbnailResponse
{
  public int Total { get; set; }
  public List<AnimatedThumbnailsResponse> Data { get; set; }
}
