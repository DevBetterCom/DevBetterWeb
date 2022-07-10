using System.Collections.Generic;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class VideoComment : BaseEntity, IAggregateRoot
{
  public string? VideoId { get; set; }
  public int? ParentCommentId { get; set; }
  public string? Body { get; set; }
  public int MemberId { get; set; }
  public Member MemberWhoCreate { get; private set; } = new Member();
  public VideoComment ParentComment { get; private set; } = new VideoComment();
  public List<VideoComment> Replies { get; private set; } = new List<VideoComment>();
  public ArchiveVideo Video { get; set; } = new ArchiveVideo();

  public void AddReplay(VideoComment comment)
  {
	  Guard.Against.Null(comment, nameof(comment));
	  Replies.Add(comment);
  }

  public void SetParentComment(VideoComment comment)
  {
	  Guard.Against.Null(comment, nameof(comment));
	  ParentComment = comment;
  }

  public void SetCreator(Member member)
  {
	  Guard.Against.Null(member, nameof(member));
	  MemberWhoCreate = member;
  }

  public override string ToString()
  {
    return Body!;
  }
}
