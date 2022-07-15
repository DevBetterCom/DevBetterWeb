using System;
using DevBetterWeb.Core.Extensions;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class Question : BaseEntity
{
  public int? ArchiveVideoId { get; private set; }
  public int MemberId { get; private set; }
  public int CoachingSessionId { get; private set; }
  public string? QuestionText { get; private set; }
  public DateTime TimestampSeconds { get; private set; }
  public CoachingSession CoachingSession { get; private set; } = new CoachingSession(DateTime.UtcNow);
	public int Votes { get; private set; }
	public Member MemberWhoCreate { get; private set; } = new Member();

	public Question(int memberId, string questionText)
	{
		MemberId = memberId;
		QuestionText = questionText;
		TimestampSeconds = DateTime.UtcNow;
	}

	public Question(int coachingSessionId, int memberId, string questionText)
  {
	  CoachingSessionId = coachingSessionId;
		MemberId = memberId;
	  QuestionText = questionText;
	  TimestampSeconds = DateTime.UtcNow;
  }

  public void UpdateQuestion(string questionText)
  {
	  QuestionText = questionText;
  }

  public void AddVote()
  {
	  Votes++;
  }

	public void SetMember(Member memberWhoCreate)
  {
	  MemberWhoCreate = memberWhoCreate;
  }

  public void SetArchiveVideoId(int archiveVideoId)
  {
	  ArchiveVideoId = archiveVideoId;
  }
}
