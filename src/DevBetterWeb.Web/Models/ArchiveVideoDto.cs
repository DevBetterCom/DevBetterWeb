using System;

namespace DevBetterWeb.Web.Models
{
  public class ArchiveVideoDto
  {
    public string? Title { get; set; }
    public string? VideoId { get; set; }
    public string? ShowNotes { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? AnimatedThumbnailUri { get; set; }
    public int Duration { get; set; }
    public int Views { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset DateUploaded { get; set; }
    public string? VideoUrl { get; set; }
  }
}
