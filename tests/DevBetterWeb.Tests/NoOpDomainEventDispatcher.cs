using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Tests;

public class NoOpDomainEventDispatcher : IDomainEventDispatcher
{
  public Task Dispatch(BaseDomainEvent domainEvent)
  {
    return Task.CompletedTask;
  }
}
