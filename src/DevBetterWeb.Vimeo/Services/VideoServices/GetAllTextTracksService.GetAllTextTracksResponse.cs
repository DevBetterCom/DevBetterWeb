using System.Collections.Generic;
using DevBetterWeb.Vimeo.Models;

namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class GetAllTextTracksResponse
{
  public int Total { get; set; }
  public List<TextTrack> Data { get; set; }
}
