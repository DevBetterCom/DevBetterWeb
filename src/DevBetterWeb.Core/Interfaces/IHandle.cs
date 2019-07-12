using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Interfaces
{
    public interface IHandle<T> where T : BaseDomainEvent
    {
        void Handle(T domainEvent);
    }
}