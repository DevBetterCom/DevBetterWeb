using System;
using DevBetterWeb.Core.Extensions;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class Question : BaseEntity
{
  public int? ArchiveVideoId { get; private set; }
  public int MemberId { get; private set; }
  public string? QuestionText { get; private set; }
  public long TimestampSeconds { get; private set; }
  public Member MemberWhoCreate { get; private set; } = new Member();

  public Question(int memberId, string questionText)
  {
	  MemberId = memberId;
	  QuestionText = questionText;
	  TimestampSeconds = DateTime.UtcNow.ConvertToUnixTimeSeconds();
  }

  public void UpdateQuestion(string questionText)
  {
	  QuestionText = questionText;
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
