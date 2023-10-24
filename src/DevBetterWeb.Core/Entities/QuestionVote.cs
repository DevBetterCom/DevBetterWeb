using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class QuestionVote : BaseEntity, IAggregateRoot
{
  public int QuestionId { get; private set; }
  public int MemberId { get; private set; }

  public Question? Question { get; private set; }
  public Member? Member { get; private set; }

  public QuestionVote(int memberId, int questionId)
  {
	  MemberId = memberId;
	  QuestionId = questionId;
  }
}
