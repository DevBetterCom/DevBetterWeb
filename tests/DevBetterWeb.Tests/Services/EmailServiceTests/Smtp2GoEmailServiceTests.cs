using System;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace DevBetterWeb.Tests.Services.EmailServiceTests;

public class Smtp2GoEmailServiceTests
{
  [Fact]
  public void Constructor_WithNullOptions_ThrowsArgumentNullException()
  {
    Assert.Throws<ArgumentNullException>(() => new Smtp2GoEmailService(null!));
  }

  [Fact]
  public async Task SendEmailAsync_WithNullSmtpServer_ThrowsException()
  {
    var options = new AuthMessageSenderOptions();
    var service = new Smtp2GoEmailService(Options.Create(options));

    var exception = await Assert.ThrowsAsync<Exception>(() =>
      service.SendEmailAsync("test@example.com", "Test Subject", "Test Message"));

    Assert.Equal("SMTP Server not set.", exception.Message);
  }

  [Fact]
  public async Task SendEmailAsync_WithNullSmtpUsername_ThrowsException()
  {
    var options = new AuthMessageSenderOptions
    {
      SmtpServer = "mail.smtp2go.com"
    };
    var service = new Smtp2GoEmailService(Options.Create(options));

    var exception = await Assert.ThrowsAsync<Exception>(() =>
      service.SendEmailAsync("test@example.com", "Test Subject", "Test Message"));

    Assert.Equal("SMTP Username not set.", exception.Message);
  }

  [Fact]
  public async Task SendEmailAsync_WithNullSmtpPassword_ThrowsException()
  {
    var options = new AuthMessageSenderOptions
    {
      SmtpServer = "mail.smtp2go.com",
      SmtpUsername = "testuser"
    };
    var service = new Smtp2GoEmailService(Options.Create(options));

    var exception = await Assert.ThrowsAsync<Exception>(() =>
      service.SendEmailAsync("test@example.com", "Test Subject", "Test Message"));

    Assert.Equal("SMTP Password not set.", exception.Message);
  }
}