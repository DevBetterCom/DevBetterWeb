using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Core.SharedKernel;

public abstract class HasDomainEventsBase : IHasDomainEvents
{
	protected readonly List<BaseDomainEvent> _domainEvents = new();
	[NotMapped]
	public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

	protected void RegisterDomainEvent(BaseDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
	internal void ClearDomainEvents() => _domainEvents.Clear();
}
