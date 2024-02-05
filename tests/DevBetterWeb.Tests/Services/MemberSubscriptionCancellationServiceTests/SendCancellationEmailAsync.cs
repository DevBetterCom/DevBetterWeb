using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionCancellationServiceTests;

public class SendCancellationEmailAsync : MemberSubscriptionCancellationServiceTest
{
  private readonly string _email = "TestEmail";

  [Fact]
  public async Task SendsEmail()
  {
    await _memberCancellationService.SendCancellationEmailAsync(_email);

    await _emailService.Received().SendEmailAsync(_email, Arg.Any<string>(), Arg.Any<string>());
  }
}


