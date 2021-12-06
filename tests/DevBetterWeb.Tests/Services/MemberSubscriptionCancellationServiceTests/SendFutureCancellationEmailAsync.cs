using System;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Specs;
using Moq;
using Xunit;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionCancellationServiceTests;

public class SendFutureCancellationEmailAsync : MemberSubscriptionCancellationServiceTest
{
  private readonly string _email = "TestEmail";
  private readonly DateTime _date = new DateTime(1, 1, 1);

  [Fact]
  public async Task SendsEmail()
  {
    _memberRepository.Setup(s => s.GetBySpecAsync(It.IsAny<MemberByUserIdSpec>(), CancellationToken.None)).ReturnsAsync(new Member());
    _subscriptionPeriodCalculationsService.Setup(s => s.GetCurrentSubscriptionEndDate(It.IsAny<Member>())).Returns(_date);

    await _memberCancellationService.SendFutureCancellationEmailAsync(_email);

    _emailService.Verify(e => e.SendEmailAsync(_email, It.IsAny<string>(), It.IsAny<string>()));
  }
}
