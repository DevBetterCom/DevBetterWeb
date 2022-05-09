using System;

namespace DevBetterWeb.WpfUploader.ViewModels;

public class GridDataVideo
{
  public string? RowNumber { get; set; }
  public string? Title { get; set; }
  public string? VideoId { get; set; }
  public string? ShowNotes { get; set; }
  public string? Description { get; set; }
  public int Duration { get; set; }
  public string? AnimatedThumbnailUri { get; set; }
  public string? DateCreated { get; set; }
  public string? DateUploaded { get; set; }
  public string? VideoUrl { get; set; }
  public string? Status { get; set; }
  public bool IsUploaded { get; set; }
}
