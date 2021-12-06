using System.Threading.Tasks;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Interfaces;

public interface IHandle<T> where T : BaseDomainEvent
{
  Task Handle(T domainEvent);
}
