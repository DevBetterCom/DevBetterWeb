using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IUserRoleMembershipService
  {
    Task AddUserToRoleAsync(string userId, string roleId);
    Task RemoveUserFromRoleAsync(string userId, string roleId);
    Task AddUserToRoleAsyncByRoleName(string userId, string roleName);
    }
}
