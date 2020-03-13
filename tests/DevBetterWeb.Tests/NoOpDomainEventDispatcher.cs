using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using System.Threading.Tasks;

namespace DevBetterWeb.Tests
{
    public class NoOpDomainEventDispatcher : IDomainEventDispatcher
    {
        public Task Dispatch(BaseDomainEvent domainEvent)
        {
            return Task.CompletedTask;
        }
    }
}
