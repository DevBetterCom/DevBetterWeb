using System;

namespace DevBetterWeb.Web.Models;

public class CoachingSessionDto
{
  public int? Id { get; set; }
  public DateTime StartAt { get; set; }
	public bool IsActive { get; set; }
}
