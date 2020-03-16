using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Infrastructure.Services
{
    /// <summary>
    /// Used by Forgot Password and similar Identity features
    /// </summary>
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridEmailService _emailService;

        public SendGridEmailSender(SendGridEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
           await _emailService.SendEmailAsync(email, subject, htmlMessage);
        }
    }
}
