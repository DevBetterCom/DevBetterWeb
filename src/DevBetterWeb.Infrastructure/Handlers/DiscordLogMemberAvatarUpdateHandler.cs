using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogMemberAvatarUpdateHandler : IHandle<MemberAvatarUpdatedEvent>
{
  private readonly DevBetterComNotificationsWebhook _webhook;

  public DiscordLogMemberAvatarUpdateHandler(DevBetterComNotificationsWebhook webhook)
  {
    _webhook = webhook;
  }

  public Task Handle(MemberAvatarUpdatedEvent memberAvatarUpdatedEvent)
  {
    _webhook.Content = returnWebhookMessageString(memberAvatarUpdatedEvent);
    return _webhook.Send();
  }

  public static string returnWebhookMessageString(MemberAvatarUpdatedEvent memberAvatarUpdatedEvent)
  {
    return $"User {memberAvatarUpdatedEvent.Member.FirstName} {memberAvatarUpdatedEvent.Member.LastName} just updated their avatar. " +
        $"Check it out here: https://devbetter.com/User/Details/{memberAvatarUpdatedEvent.Member.UserId}.";
  }
}
