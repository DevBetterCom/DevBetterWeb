using System.Collections.Generic;
using DevBetterWeb.Vimeo.Models;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetAllAnimatedThumbnailResponse
{
  public int Total { get; set; }
  public List<AnimatedThumbnailsResponse> Data { get; set; }
}
