using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IUserEmailConfirmationService
  {
    Task UpdateUserEmailConfirmationAsync(string userId, bool isEmailConfirmed);
  }
}
