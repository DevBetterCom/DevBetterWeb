namespace DevBetterWeb.Web.Models;

public class NewQuestionRequestDto
{
	public int CoachingSessionId { get; set; }
	public string QuestionText { get; set; } = string.Empty;
}
