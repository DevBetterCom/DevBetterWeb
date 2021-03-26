using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IUserRoleMembershipService
  {
    Task AddUserToRoleAsync(string userId, string roleId);
    Task RemoveUserFromRoleAsync(string userId, string roleId);
    Task AddUserToRoleByRoleNameAsync(string userId, string roleName);
    Task RemoveUserFromRoleByRoleNameAsync(string userId, string roleName);
    }
}
