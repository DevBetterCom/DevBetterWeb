using Xunit;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionPeriodCalculationsServiceTests
{
  public class GetPercentageProgressToAlumniStatus
  {
    private readonly IMemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;

    public GetPercentageProgressToAlumniStatus()
    {
      _memberSubscriptionPeriodCalculationsService = new MemberSubscriptionPeriodCalculationsService();
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(73)]
    [InlineData(730)]
    [InlineData(63)]
    public void CalculatesCorrectPercentageGivenPercentageBelow100(int days)
    {
      var member = MemberHelpers.CreateWithDefaultConstructor();
      var subscription = SubscriptionHelpers.GetSubscriptionWithGivenSubscribedDaysToDateAndTotalSubscribedDays(days);
      member.AddSubscription(subscription.Dates, 1);

      var expectedPercentage = (int) (100 * ((double)days / (double) 730));

      var percentage = _memberSubscriptionPeriodCalculationsService.GetPercentageProgressToAlumniStatus(member);

      Assert.Equal(expectedPercentage, percentage);
    }

    [Fact]
    public void Returns100GivenPercentageOver100()
    {
      var member = MemberHelpers.CreateWithDefaultConstructor();
      var subscription = SubscriptionHelpers.GetSubscriptionWithGivenSubscribedDaysToDateAndTotalSubscribedDays(731);
      member.AddSubscription(subscription.Dates, 1);

      var expectedPercentage = 100;

      var percentage = _memberSubscriptionPeriodCalculationsService.GetPercentageProgressToAlumniStatus(member);

      Assert.Equal(expectedPercentage, percentage);
    }
  }
}
