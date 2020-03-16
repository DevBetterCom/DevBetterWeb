using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace DevBetterWeb.Infrastructure.Services
{
    /// <summary>
    /// Used by Forgot Password and similar Identity features
    /// </summary>
    public class SendGridEmailSender : IEmailSender
    {
        private readonly IEmailService _emailService;

        public SendGridEmailSender(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
           await _emailService.SendEmailAsync(email, subject, htmlMessage);
        }
    }
}
