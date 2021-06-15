using System.Threading.Tasks;
using Xunit;
using Moq;
using System;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionCancellationServiceTests
{
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

}


