using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Tests.Core.Entities.MemberTests;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionPeriodCalculationsServiceTests
{
  public class GetPercentageProgressToAlumniStatus
  {
    private readonly IMemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;

    public GetPercentageProgressToAlumniStatus()
    {
      _memberSubscriptionPeriodCalculationsService = new MemberSubscriptionPeriodCalculationsService();
    }

    [Fact]
    public void CalculatesCorrectPercentageGivenPercentageBelow100()
    {
      var member = MemberHelpers.CreateWithDefaultConstructor();
      var subscription = SubscriptionHelpers.GetSubscriptionWithGivenSubscribedDaysToDateAndTotalSubscribedDays(73);
      member.AddSubscription(subscription);

      var expectedPercentage = 100 * 73 / 730;

      var percentage = _memberSubscriptionPeriodCalculationsService.GetPercentageProgressToAlumniStatus(member);

      Assert.Equal(expectedPercentage, percentage);
    }

    [Fact]
    public void Returns100GivenPercentageOver100()
    {
      var member = MemberHelpers.CreateWithDefaultConstructor();
      var subscription = SubscriptionHelpers.GetSubscriptionWithGivenSubscribedDaysToDateAndTotalSubscribedDays(731);
      member.AddSubscription(subscription);

      var expectedPercentage = 100;

      var percentage = _memberSubscriptionPeriodCalculationsService.GetPercentageProgressToAlumniStatus(member);

      Assert.Equal(expectedPercentage, percentage);
    }
  }
}
