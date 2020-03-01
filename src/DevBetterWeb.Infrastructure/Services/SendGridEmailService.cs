using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace DevBetterWeb.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        public SendGridEmailService(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            // TODO: Add Guard.AgainstNUll(optionsAccessor.Value)
            Options = optionsAccessor.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (Options.SendGridKey == null) throw new Exception("SendGridKey not set.");
            var response = await Execute(Options.SendGridKey, subject, message, email);

            if(response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                // log or throw
                throw new Exception("Could not send email: " + await response.Body.ReadAsStringAsync());
            }
        }

        private async Task<Response> Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("donotreply@devbetter.com", "devBetter Admin"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return await client.SendEmailAsync(msg);
        }
    }
}
