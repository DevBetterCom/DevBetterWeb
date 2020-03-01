using DevBetterWeb.Core.SharedKernel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
    // TODO: Make async
    public interface IRepository
    {
        Task<T> GetByIdAsync<T>(int id) where T : BaseEntity;
        List<T> List<T>() where T : BaseEntity;
        T Add<T>(T entity) where T : BaseEntity;
        void Update<T>(T entity) where T : BaseEntity;
        void Delete<T>(T entity) where T : BaseEntity;
    }
}
