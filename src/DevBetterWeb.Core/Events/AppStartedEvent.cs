using System;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class AppStartedEvent : BaseDomainEvent
{
  public AppStartedEvent(DateTime startDateTime)
  {
    StartDateTime = startDateTime;
  }

  public DateTime StartDateTime { get; }
}
