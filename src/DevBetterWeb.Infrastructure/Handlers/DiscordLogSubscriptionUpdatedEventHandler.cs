using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Infrastructure.Handlers;

public class DiscordLogSubscriptionUpdatedEventHandler : IHandle<SubscriptionUpdatedEvent>
{
  private readonly AdminUpdatesWebhook _webhook;
  
  public DiscordLogSubscriptionUpdatedEventHandler(AdminUpdatesWebhook webhook)
  {
    _webhook = webhook;
  }
  
  public Task Handle(SubscriptionUpdatedEvent domainEvent)
  {
	  var message = $"Member {domainEvent.Member.UserFullName()} updated subscription {domainEvent.MemberSubscription.Id}";
    return _webhook.SendAsync(message);
  }
}
