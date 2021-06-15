using System.Threading.Tasks;

namespace DevBetterWeb.Core.Interfaces
{
  public interface IMemberSubscriptionEndedAdminEmailService
  {
    Task SendMemberSubscriptionEndedEmailAsync(string customerEmail);
  }
}
