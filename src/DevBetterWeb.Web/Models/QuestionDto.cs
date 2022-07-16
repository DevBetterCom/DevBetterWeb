using System;

namespace DevBetterWeb.Web.Models;

public class QuestionDto
{
	public int? ArchiveVideoId { get; set; }
	public int MemberId { get; set; }
	public int CoachingSessionId { get; set; }
	public string QuestionText { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public int Votes { get; set; }
	public string MemberName { get; set; } = string.Empty;
}
