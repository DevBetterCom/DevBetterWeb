using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces;

public interface IMemberCancellationService
{
  Task RemoveUserFromMemberRoleAsync(string email);
  // for immediately after member cancels
  Task SendFutureCancellationEmailAsync(string email);
  // for immediately after member cancellation takes effect
  Task SendCancellationEmailAsync(string email);
}
