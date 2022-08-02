using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class VideoComment : BaseEntity, IAggregateRoot
{
  public int VideoId { get; set; }
  public int? ParentCommentId { get; set; }
  public string? MdBody { get; set; }

  public string? Body { get; set; }
  public int MemberId { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public Member? MemberWhoCreate { get; private set; }
  public VideoComment? ParentComment { get; private set; }
  public List<VideoComment> Replies { get; private set; } = new List<VideoComment>();
  public ArchiveVideo? Video { get; set; }

  public VideoComment()
  {
  }
  public VideoComment(int memberId, int videoId, string body)
  {
	  MemberId = memberId;
	  Body = body;
	  VideoId = videoId;
	  CreatedAt = DateTime.Now;
  }

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

  public void CreateMdBody(IMarkdownService markdownService)
  {
	  MdBody = markdownService.RenderHTMLFromMD(Body);
  }

  public override string ToString()
  {
    return Body!;
  }
}
