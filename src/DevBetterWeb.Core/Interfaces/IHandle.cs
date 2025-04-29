using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.SharedKernel;
using MediatR;

namespace DevBetterWeb.Core.Interfaces;

public interface IHandle<in T> where T : BaseDomainEvent
{
	Task Handle(T domainEvent, CancellationToken cancellationToken);
}
