using System;
using DevBetterWeb.Core.SharedKernel;
using MediatR;

namespace DevBetterWeb.Core.Events;

public class AppStartedEvent : BaseDomainEvent, INotification
{
  public AppStartedEvent(DateTime startDateTime)
  {
    StartDateTime = startDateTime;
  }

  public DateTime StartDateTime { get; }
}
