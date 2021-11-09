using Ardalis.Specification;

namespace DevBetterWeb.Core.Interfaces;

public interface IRepository<T> : IRepositoryBase<T>, IReadRepositoryBase<T> where T : class, IAggregateRoot
{
}
