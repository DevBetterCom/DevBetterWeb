﻿using System;
using System.Collections.Generic;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class CoachingSession : BaseEntity
{
  public DateTime StartAt { get; private set; }
  public List<Question> Questions { get; private set; } = new List<Question>();

  public CoachingSession(DateTime startAt)
  {
	  StartAt = startAt;
  }

  public void AddQuestion(Question question)
  {
	  Questions.Add(question);
  }

  public void UpdateStart(DateTime startAt)
  {
	  StartAt = startAt;
  }
}
