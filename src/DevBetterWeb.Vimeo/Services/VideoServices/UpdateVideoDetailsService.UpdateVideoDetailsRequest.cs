namespace DevBetterWeb.Vimeo.Services.VideoServices;

public class UploadTextTrackFileRequest
{
  public string Link { get; set; }
  public byte[] FileData { get; set; }

  public UploadTextTrackFileRequest(string link, byte[] fileData)
  {
    Link = link;
    FileData = fileData;
  }
}
