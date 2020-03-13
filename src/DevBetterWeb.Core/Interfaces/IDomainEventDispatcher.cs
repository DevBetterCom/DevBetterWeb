using DevBetterWeb.Core.SharedKernel;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch(BaseDomainEvent domainEvent);
    }
}