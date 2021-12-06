using System.Threading.Tasks;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Interfaces;

public interface IDomainEventDispatcher
{
  Task Dispatch(BaseDomainEvent domainEvent);
}
