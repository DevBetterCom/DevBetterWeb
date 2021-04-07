using System.Threading.Tasks;
using Xunit;
using Moq;
using System;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionCancellationServiceTests
{
  public class SendFutureCancellationEmailAsync : MemberSubscriptionCancellationServiceTest
  {
    private readonly string _email = "TestEmail";
    private readonly DateTime _date = new DateTime(1, 1, 1);

    [Fact]
    public async Task SendsEmail()
    {
      _subscriptionPeriodCalculationsService.Setup(s => s.GetCurrentSubscriptionEndDate(It.IsAny<Member>())).Returns(_date);

      await _memberCancellationService.SendFutureCancellationEmailAsync(_email);

      _emailService.Verify(e => e.SendEmailAsync(_email, It.IsAny<string>(), It.IsAny<string>()));
    }
  }

}


