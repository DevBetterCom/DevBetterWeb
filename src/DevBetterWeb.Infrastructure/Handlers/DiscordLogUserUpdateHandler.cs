using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Handlers
{
    public class DiscordLogUserUpdateHandler : IHandle<MemberUpdatedEvent>
    {
        private readonly Webhook _webhook;

        public DiscordLogUserUpdateHandler(Webhook webhook)
        {
            _webhook = webhook;
        }

        public async Task Handle(MemberUpdatedEvent memberUpdatedEvent)
        {
            _webhook.Content = returnWebhookMessageString(memberUpdatedEvent);
            await _webhook.Send();
        }

        public static string returnWebhookMessageString(MemberUpdatedEvent memberUpdatedEvent)
        {
            return $"User {memberUpdatedEvent.Member.FirstName} {memberUpdatedEvent.Member.LastName} just updated their profile. " +
                $"Check it out here: https://devbetter.com/User/Details/{memberUpdatedEvent.Member.Id}.";
        }
    }
}
