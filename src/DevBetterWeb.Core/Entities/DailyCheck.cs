using System;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class DailyCheck : BaseEntity, IAggregateRoot
{
  public DateTime Date { get; set; }
  public string? TasksCompleted { get; set; }
}
