using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Core.Handlers;

public class DiscordLogMemberUpdateHomeAddressHandler : IHandle<MemberHomeAddressUpdatedEvent>
{
  private readonly AdminUpdatesWebhook _webhook;

  public DiscordLogMemberUpdateHomeAddressHandler(AdminUpdatesWebhook webhook)
  {
    _webhook = webhook;
  }

  public Task Handle(MemberHomeAddressUpdatedEvent memberHomeAddressUpdatedEvent, CancellationToken cancellationToken)
  {
    var message = returnWebhookMessageString(memberHomeAddressUpdatedEvent);
    return _webhook.SendAsync(message);
  }

  public static string returnWebhookMessageString(MemberHomeAddressUpdatedEvent memberHomeAddressUpdatedEvent)
  {
    return $"User {memberHomeAddressUpdatedEvent.Member.FirstName} {memberHomeAddressUpdatedEvent.Member.LastName} just updated their profile. " +
					 $"Home Address to {memberHomeAddressUpdatedEvent.UpdateDetails}. " +
        $"Check it out here: https://devbetter.com/User/Details/{memberHomeAddressUpdatedEvent.Member.UserId} ";
  }
}
