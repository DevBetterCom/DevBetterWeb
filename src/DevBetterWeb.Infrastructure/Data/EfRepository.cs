using Ardalis.Specification.EntityFrameworkCore;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Infrastructure.Data
{
  public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
  {
    public EfRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
  }
}
