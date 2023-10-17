using System.Threading.Tasks;
using Moq;
using Xunit;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionCancellationServiceTests;

public class SendCancellationEmailAsync : MemberSubscriptionCancellationServiceTest
{
  private readonly string _email = "TestEmail";

  [Fact]
  public async Task SendsEmail()
  {
    await _memberCancellationService.SendCancellationEmailAsync(_email);

    _emailService.Verify(e => e.SendEmailAsync(_email, It.IsAny<string>(), It.IsAny<string>()));
  }
}


