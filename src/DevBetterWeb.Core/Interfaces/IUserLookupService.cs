using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IUserLookupService
  {
    Task<string> FindUserIdByEmailAsync(string email);
    Task<bool> FindUserIsMemberByEmailAsync(string email);
    Task<bool> FindUserIsAlumniByUserIdAsync(string userId);
  }
}
