using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Interfaces
{
    public interface IDomainEventDispatcher
    {
        void Dispatch(BaseDomainEvent domainEvent);
    }
}