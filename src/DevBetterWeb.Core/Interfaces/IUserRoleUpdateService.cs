using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
    public interface IUserRoleUpdateService
    {
        Task AddUserToRoleAsync(string userId, string roleId);
        Task RemoveUserFromRoleAsync(string userId, string roleId);
    }
}
