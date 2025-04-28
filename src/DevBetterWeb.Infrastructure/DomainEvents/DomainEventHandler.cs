using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using MediatR;

namespace DevBetterWeb.Infrastructure.DomainEvents;

public class DomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
	where TDomainEvent : BaseDomainEvent
{
	private readonly IEnumerable<IHandle<TDomainEvent>> _handlers;

	public DomainEventHandler(IEnumerable<IHandle<TDomainEvent>> handlers)
	{
		_handlers = handlers;
	}

	public async Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
	{
		foreach (var handler in _handlers)
		{
			await handler.Handle(notification, cancellationToken);
		}
	}
}
