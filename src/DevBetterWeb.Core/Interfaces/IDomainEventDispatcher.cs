using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces;

public interface IDomainEventDispatcher
{
	Task DispatchAndClearEvents(IEnumerable<IHasDomainEvents> entitiesWithEvents);
}
