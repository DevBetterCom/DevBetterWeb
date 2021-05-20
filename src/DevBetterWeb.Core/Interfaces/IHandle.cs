using DevBetterWeb.Core.SharedKernel;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IHandle<T> where T : BaseDomainEvent
  {
    Task Handle(T domainEvent);
  }
}
