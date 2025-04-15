using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Infrastructure.DomainEvents;

public class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
	private readonly IMediator _mediator;
	private readonly ILogger<MediatRDomainEventDispatcher> _logger;

	public MediatRDomainEventDispatcher(IMediator mediator, ILogger<MediatRDomainEventDispatcher> logger)
	{
		_mediator = mediator;
		_logger = logger;
	}

	public async Task DispatchAndClearEvents(IEnumerable<IHasDomainEvents> entitiesWithEvents)
	{
		foreach (var entity in entitiesWithEvents)
		{
			if (entity is HasDomainEventsBase hasDomainEvents)
			{
				BaseDomainEvent[] events = hasDomainEvents.DomainEvents.ToArray();
				hasDomainEvents.ClearDomainEvents();

				foreach (BaseDomainEvent domainEvent in events)
					await _mediator.Publish(domainEvent).ConfigureAwait(false);
			}
			else
			{
				_logger.LogError(
					"Entity of type {EntityType} does not inherit from {BaseType}. Unable to clear domain events.",
					entity.GetType().Name,
					nameof(HasDomainEventsBase));
			}
		}
	}
}
