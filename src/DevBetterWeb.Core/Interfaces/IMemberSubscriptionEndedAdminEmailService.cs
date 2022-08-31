using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Core.Interfaces;

public interface IMemberSubscriptionEndedAdminEmailService
{
  Task SendMemberSubscriptionEndedEmailAsync(string customerEmail, Member? memberFullInfo);
}
