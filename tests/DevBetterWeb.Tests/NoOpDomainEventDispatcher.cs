using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Tests
{
    public class NoOpDomainEventDispatcher : IDomainEventDispatcher
    {
        public void Dispatch<TEvent>(TEvent domainEvent) where TEvent : BaseDomainEvent
        {
        }
    }
}
