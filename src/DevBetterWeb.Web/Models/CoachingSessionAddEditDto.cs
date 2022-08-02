using System;

namespace DevBetterWeb.Web.Models;

public class CoachingSessionAddEditDto
{
  public int? Id { get; set; }
  public string StartAt { get; set; } = String.Empty;
}
