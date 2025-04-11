using System.Collections.Generic;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Interfaces;

public interface IHasDomainEvents
{
	IReadOnlyCollection<BaseDomainEvent> DomainEvents { get; }
}
