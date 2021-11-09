using System.Net.Mail;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Infrastructure.Services;

public class LocalSmtpEmailService : IEmailService
{
  public async Task SendEmailAsync(string email, string subject, string message)
  {
    using var client = new SmtpClient("localhost");
    var mailMessage = new MailMessage("donotreply@devbetter.com", email, subject, message);
    await client.SendMailAsync(mailMessage);
  }
}
