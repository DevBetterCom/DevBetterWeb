using DevBetterWeb.Vimeo.Models;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class UploadVideoRequest
  {

    public string UserId { get; set; }
    public byte[] FileData { get; set; }
    public Video Video { get; set; }
  }
}
