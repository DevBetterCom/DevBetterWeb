using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.DiscordWebooks;

namespace DevBetterWeb.Infrastructure.Handlers;

public class DiscordLogBillingActivityCreatedEventHandler : IHandle<BillingActivityCreatedEvent>
{
  private readonly AdminUpdatesWebhook _webhook;
  
  public DiscordLogBillingActivityCreatedEventHandler(AdminUpdatesWebhook webhook)
  {
  	_webhook = webhook;
  }
  
  public Task Handle(BillingActivityCreatedEvent domainEvent)
  {
  	_webhook.Content = $"New BillingActivity created for member {domainEvent.BillingActivity.Details.MemberName} with action {domainEvent.BillingActivity.Details.ActionVerbPastTense}";
  	return _webhook.Send();
  }
}
