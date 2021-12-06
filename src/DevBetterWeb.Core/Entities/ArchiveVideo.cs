using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class ArchiveVideo : BaseEntity, IAggregateRoot
{
  public string? Title { get; set; }
  public string? VideoId { get; set; }
  public string? ShowNotes { get; set; }
  public string? Description { get; set; }
  public int Duration { get; set; }
  public DateTimeOffset DateCreated { get; set; }
  public DateTimeOffset DateUploaded { get; set; }
  public string? VideoUrl { get; set; }
  public string? Status { get; set; }
  public string? AnimatedThumbnailUri { get; set; }
  public int Views { get; set; } = 0;

  public List<Question> Questions { get; private set; } = new List<Question>();

  public void AddQuestion(Question question)
  {
    Guard.Against.Null(question, nameof(question));
    Questions.Add(question);
  }
}
