using System.Collections.Generic;
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

	public Task DispatchAndClearEvents(IEnumerable<IHasDomainEvents> entitiesWithEvents)
	{
		return Task.CompletedTask;
	}
}
