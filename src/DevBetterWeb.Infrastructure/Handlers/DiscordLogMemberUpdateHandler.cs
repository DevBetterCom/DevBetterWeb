using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;

namespace DevBetterWeb.Core.Handlers
{
  public class DiscordLogMemberUpdateHandler : IHandle<MemberUpdatedEvent>
  {
    private readonly DevBetterComNotificationsWebhook _webhook;

    public DiscordLogMemberUpdateHandler(DevBetterComNotificationsWebhook webhook)
    {
      _webhook = webhook;
    }

    public Task Handle(MemberUpdatedEvent memberUpdatedEvent)
    {
      _webhook.Content = returnWebhookMessageString(memberUpdatedEvent);
      return _webhook.Send();
    }

    public static string returnWebhookMessageString(MemberUpdatedEvent memberUpdatedEvent)
    {
      return $"User {memberUpdatedEvent.Member.FirstName} {memberUpdatedEvent.Member.LastName} just updated their profile. " +
          $"Check it out here: https://devbetter.com/User/Details/{memberUpdatedEvent.Member.UserId}.";
    }
  }


}
