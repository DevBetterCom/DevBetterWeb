using System;

namespace DevBetterWeb.UploaderApp;

public class ArchiveVideo
{
  public string? Title { get; set; }
  public string? VideoId { get; set; }
  public string? ShowNotes { get; set; }
  public string? Description { get; set; }
  public string? Password { get; set; }
  public int Duration { get; set; }
  public string? AnimatedThumbnailUri { get; set; }
  public DateTimeOffset DateCreated { get; set; }
  public DateTimeOffset DateUploaded { get; set; }
  public string? VideoUrl { get; set; }
  public string? Status { get; set; }
}
