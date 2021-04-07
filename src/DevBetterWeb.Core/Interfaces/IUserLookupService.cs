using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IUserLookupService
  {
    Task<string> FindUserIdByEmailAsync(string email);
  }
}
