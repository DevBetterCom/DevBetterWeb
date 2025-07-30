using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Infrastructure.Services;

public class Smtp2GoEmailService : IEmailService
{
  public Smtp2GoEmailService(IOptions<AuthMessageSenderOptions> optionsAccessor)
  {
    Guard.Against.Null(optionsAccessor.Value, nameof(optionsAccessor.Value));
    Options = optionsAccessor.Value;
  }

  public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

  public async Task SendEmailAsync(string email, string subject, string message)
  {
    if (string.IsNullOrEmpty(Options.SmtpServer)) throw new Exception("SMTP Server not set.");
    if (string.IsNullOrEmpty(Options.SmtpUsername)) throw new Exception("SMTP Username not set.");
    if (string.IsNullOrEmpty(Options.SmtpPassword)) throw new Exception("SMTP Password not set.");

    using var client = new SmtpClient(Options.SmtpServer, Options.SmtpPort)
    {
      EnableSsl = true,
      Credentials = new NetworkCredential(Options.SmtpUsername, Options.SmtpPassword)
    };

    var mailMessage = new MailMessage(
      from: "donotreply@devbetter.com",
      to: email,
      subject: subject,
      body: message)
    {
      IsBodyHtml = true
    };

    await client.SendMailAsync(mailMessage);
  }
}