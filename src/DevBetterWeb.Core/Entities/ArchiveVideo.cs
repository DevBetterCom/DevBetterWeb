using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Entities;
public class ArchiveVideo : BaseEntity, IAggregateRoot
{
  public string? Title { get; set; }
  public string? VideoId { get; set; }
  public string? Description { get; set; }
  public int Duration { get; set; }
  public DateTimeOffset DateCreated { get; set; }
  public DateTimeOffset DateUploaded { get; set; }
  public string? VideoUrl { get; set; }
  public string? Status { get; set; }
  public string? AnimatedThumbnailUri { get; set; }
  public int Views { get; set; } = 0;

  public List<Question> Questions { get; private set; } = new List<Question>();
  public List<MemberVideoProgress> MembersVideoProgress { get; set; } = new List<MemberVideoProgress>();
  public List<VideoComment> Comments { get; private set; } = new List<VideoComment>();


  private readonly List<MemberFavoriteArchiveVideo> _memberFavorites = new();
  public IEnumerable<MemberFavoriteArchiveVideo> MemberFavorites => _memberFavorites.AsReadOnly();

  public void AddQuestion(Question question)
  {
    Guard.Against.Null(question, nameof(question));
    Questions.Add(question);
  }


  public void AddVideoProgress(MemberVideoProgress memberVideoProgress)
  {
	  Guard.Against.Null(memberVideoProgress, nameof(memberVideoProgress));
	  MembersVideoProgress.Add(memberVideoProgress);
  }
  public void AddComment(VideoComment comment)
  {
	  Guard.Against.Null(comment, nameof(comment));
	  Comments.Add(comment);
  }

  public void CreateMdComments(IMarkdownService markdownService)
  {
	  foreach (var comment in Comments)
	  {
		  comment.CreateMdBody(markdownService);

	  }
  }
}
