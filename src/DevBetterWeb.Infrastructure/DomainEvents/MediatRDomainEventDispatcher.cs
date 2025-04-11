//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Autofac;
//using DevBetterWeb.Core.Interfaces;
//using DevBetterWeb.Core.SharedKernel;

//namespace DevBetterWeb.Infrastructure.DomainEvents;

//// https://gist.github.com/jbogard/54d6569e883f63afebc7
//// http://lostechies.com/jimmybogard/2014/05/13/a-better-domain-events-pattern/
//public class DomainEventDispatcher : IDomainEventDispatcher
//{
//  private readonly IComponentContext _container;

//  public DomainEventDispatcher(IComponentContext container)
//  {
//    _container = container;
//  }

//  public async Task Dispatch(BaseDomainEvent domainEvent)
//  {
//    var wrappedHandlers = GetWrappedHandlers(domainEvent);

//    foreach (DomainEventHandler handler in wrappedHandlers)
//    {
//      await handler.Handle(domainEvent).ConfigureAwait(false);
//    }
//  }

//#nullable disable
//  public IEnumerable<DomainEventHandler> GetWrappedHandlers(BaseDomainEvent domainEvent)
//  {
//    Type handlerType = typeof(IHandle<>).MakeGenericType(domainEvent.GetType());
//    Type wrapperType = typeof(DomainEventHandler<>).MakeGenericType(domainEvent.GetType());
//    IEnumerable handlers = (IEnumerable)_container.Resolve(typeof(IEnumerable<>).MakeGenericType(handlerType));
//    IEnumerable<DomainEventHandler> wrappedHandlers = handlers.Cast<object>()
//        .Select(handler => (DomainEventHandler)Activator.CreateInstance(wrapperType, handler));
//#nullable enable
//    return wrappedHandlers;
//  }

//  public abstract class DomainEventHandler
//  {
//    public abstract Task Handle(BaseDomainEvent domainEvent);
//  }

//  public class DomainEventHandler<T> : DomainEventHandler
//      where T : BaseDomainEvent
//  {
//    private readonly IHandle<T> _handler;

//    public DomainEventHandler(IHandle<T> handler)
//    {
//      _handler = handler;
//    }

//    public override Task Handle(BaseDomainEvent domainEvent)
//    {
//      return _handler.Handle((T)domainEvent);
//    }
//  }
//}
using DevBetterWeb.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using DevBetterWeb.Core.SharedKernel;
using System.Linq;

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
