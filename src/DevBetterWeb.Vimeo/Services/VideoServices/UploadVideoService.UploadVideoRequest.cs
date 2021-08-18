using DevBetterWeb.Vimeo.Models;

namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class UploadVideoRequest
  {

    public string UserId { get; set; }
    public byte[] FileData { get; set; }
    public Video Video { get; set; } = new Video();

    public UploadVideoRequest(string userId, byte[] fileData, Video video)
    {
      UserId = userId;
      FileData = fileData;
      Video = video;
    }
  }
}
