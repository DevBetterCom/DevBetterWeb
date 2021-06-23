using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IUserRoleManager
  {
    Task AddUserToRoleAsync(string userId, string roleName);
  }
}
