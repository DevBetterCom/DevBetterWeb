namespace DevBetterWeb.Vimeo.Services.VideoServices
{
  public class CompleteUploadRequest
  {
    public string UserId { get; set; } 
    public string UploadId { get; set; }
    public string CompleteUri { get; set; }
  }
}
