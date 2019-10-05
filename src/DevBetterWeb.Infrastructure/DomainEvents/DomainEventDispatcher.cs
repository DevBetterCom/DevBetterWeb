using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Infrastructure.DomainEvents
{
    // https://gist.github.com/jbogard/54d6569e883f63afebc7
    // http://lostechies.com/jimmybogard/2014/05/13/a-better-domain-events-pattern/
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly ILifetimeScope _scope;

        public DomainEventDispatcher(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public void Dispatch<TEvent>(TEvent domainEvent) where TEvent : BaseDomainEvent
        {
            var handlers = _scope.Resolve<IEnumerable<IHandle<TEvent>>>().ToList();
            handlers.ForEach(handler => handler.Handle(domainEvent));
        }

        private abstract class DomainEventHandler
        {
            public abstract void Handle(BaseDomainEvent domainEvent);
        }

        private class DomainEventHandler<T> : DomainEventHandler
            where T : BaseDomainEvent
        {
            private readonly IHandle<T> _handler;

            public DomainEventHandler(IHandle<T> handler)
            {
                _handler = handler;
            }

            public override void Handle(BaseDomainEvent domainEvent)
            {
                _handler.Handle((T)domainEvent);
            }
        }
    }
}