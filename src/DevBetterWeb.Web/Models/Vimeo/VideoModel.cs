using System;
using System.Collections.Generic;

namespace DevBetterWeb.Web.Models.Vimeo
{
  public class VideoModel
  {
    public string? Id { get; set;  }
    public DateTime? CreatedTime { get; set; }
    public string? Description { get; set; }
    public int? Duration { get; set; }
    public int? Height { get; set; }
    public DateTime? ModifiedTime { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public DateTime? ReleaseTime { get; set; }
    public string? Status { get; set; }
    public int? Width { get; set; }
  }
}
