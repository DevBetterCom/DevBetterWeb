using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Infrastructure.Handlers;

public class DiscordLogSubscriptionAddedEventHandler : IHandle<SubscriptionAddedEvent>
{
  private readonly AdminUpdatesWebhook _webhook;
  
  public DiscordLogSubscriptionAddedEventHandler(AdminUpdatesWebhook webhook)
  {
    _webhook = webhook;
  }
  
  public Task Handle(SubscriptionAddedEvent domainEvent, CancellationToken cancellationToken)
  {
	  var message = $"Member {domainEvent.Member.UserFullName()} added subscription {domainEvent.MemberSubscription.Id}";
    return _webhook.SendAsync(message);
  }
}
