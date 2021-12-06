using System;
using System.ComponentModel.DataAnnotations;

namespace DevBetterWeb.Web.Models.Vimeo;

public class VideoModel
{
  public string? Id { get; set; }
  public string? Description { get; set; }
  public int? Duration { get; set; }
  public int? Height { get; set; }
  public string? Name { get; set; }
  public string? Password { get; set; }
  public string? Status { get; set; }
  public int? Width { get; set; }

  [Display(Name = "Creation Date")]
  [DisplayFormat(DataFormatString = "{0:dd MMM yyyy hh:mm}")]
  public DateTime? CreatedTime { get; set; }
  public DateTime? ModifiedTime { get; set; }
  [Display(Name = "Recording Date")]
  [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
  public DateTime? ReleaseTime { get; set; }
}
