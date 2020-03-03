using DevBetterWeb.Core.Interfaces;
using System.Threading.Tasks;

namespace DevBetterWeb.Core.Events
{
    public class NewMemberCreatedNotificationHandler : IHandle<NewMemberCreatedEvent>
    {
        private readonly IEmailService _emailService;

        public NewMemberCreatedNotificationHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Handle(NewMemberCreatedEvent domainEvent)
        {
            // TODO: Get all Admin Users to send email to
            string toAddress = "steve@ardalis.com";
            string subject = "[devBetter] New Member: " + domainEvent.Member.UserFullName();
            string body = $"A new member has registered: \n{domainEvent.Member.UserFullName()}\nID:{domainEvent.Member.UserId}\n\n{domainEvent.Member.AboutInfo}";

            await _emailService.SendEmailAsync(toAddress, subject, body);
        }
    }
}
