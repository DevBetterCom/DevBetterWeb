using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Interfaces
{
    public interface IDomainEventDispatcher
    {
        void Dispatch<TEvent>(TEvent domainEvent) where TEvent : BaseDomainEvent;
    }
}