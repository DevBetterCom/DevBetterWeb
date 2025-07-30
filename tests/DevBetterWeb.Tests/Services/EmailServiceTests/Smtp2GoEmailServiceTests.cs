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
  public async Task SendEmailAsync_WithNullApiKey_ThrowsException()
  {
		var options = new ApiMailSenderOptions();
    var service = new Smtp2GoEmailService(Options.Create(options));

    var exception = await Assert.ThrowsAsync<Exception>(() =>
      service.SendEmailAsync("test@example.com", "Test Subject", "Test Message"));

    Assert.Equal("SMTP API Key not set.", exception.Message);
  }
}
