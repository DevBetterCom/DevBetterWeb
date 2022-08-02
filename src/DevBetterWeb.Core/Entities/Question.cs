using System;
using System.Collections.Generic;
using System.Linq;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class Question : BaseEntity, IAggregateRoot
{
  public int? ArchiveVideoId { get; private set; }
  public int MemberId { get; private set; }
  public int CoachingSessionId { get; private set; }
  public string? QuestionText { get; private set; }
  public DateTime CreatedAt { get; private set; }
  public int Votes { get; private set; }
  public CoachingSession? CoachingSession { get; private set; }
	public Member? MemberWhoCreate { get; private set; }
	public List<QuestionVote> QuestionVotes { get; set; } = new List<QuestionVote>();

	public Question(int memberId, string questionText)
	{
		MemberId = memberId;
		QuestionText = questionText;
		CreatedAt = DateTime.UtcNow;
	}

	public void UpdateQuestion(string questionText)
  {
	  QuestionText = questionText;
  }

  public void AddRemoveVote(int memberId)
  {
	  var existMemberVote = QuestionVotes.FirstOrDefault(x => x.MemberId == memberId);

		if (existMemberVote != null)
	  {
		  Votes--;
		  QuestionVotes.RemoveAll(x => x.MemberId == memberId);
	  }
		else
		{
			Votes++;
			var questionVote = new QuestionVote(memberId, Id);
			QuestionVotes.Add(questionVote);
		}
  }

	public void SetMember(Member memberWhoCreate)
  {
	  MemberWhoCreate = memberWhoCreate;
  }

  public void SetArchiveVideoId(int archiveVideoId)
  {
	  ArchiveVideoId = archiveVideoId;
  }

  public bool MemberCanUpVote(int memberId)
  {
	  return QuestionVotes.All(x => x.MemberId != memberId);
  }
}
