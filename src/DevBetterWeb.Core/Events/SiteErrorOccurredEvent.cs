using System;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class SiteErrorOccurredEvent : BaseDomainEvent
{
  public SiteErrorOccurredEvent(Exception siteException)
  {
    SiteException = siteException;
  }

  public Exception SiteException { get; }
}
