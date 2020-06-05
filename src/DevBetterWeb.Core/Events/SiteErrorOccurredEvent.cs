using DevBetterWeb.Core.SharedKernel;
using System;

namespace DevBetterWeb.Core.Events
{
    public class SiteErrorOccurredEvent : BaseDomainEvent
    {
        public SiteErrorOccurredEvent(Exception siteException)
        {
            SiteException = siteException;
        }

        public Exception SiteException { get; }
    }
}
