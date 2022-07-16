using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DevBetterWeb.Web.Models;

public class CoachingSessionDto
{
  public int? Id { get; set; }
  public DateTime StartAt { get; set; }
	public bool IsActive { get; set; }
	public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
}
