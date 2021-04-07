using DevBetterWeb.Core.Interfaces;
using Moq;
using DevBetterWeb.Core.Services;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionCancellationServiceTests
{
  public class MemberSubscriptionCancellationServiceTest
  {
    internal readonly Mock<IUserRoleMembershipService> _userRoleMembershipService;
    internal readonly Mock<IEmailService> _emailService;
    internal readonly Mock<IUserLookupService> _userLookup;
    internal readonly Mock<IRepository> _repository;
    internal readonly Mock<IMemberSubscriptionPeriodCalculationsService> _subscriptionPeriodCalculationsService;

    internal readonly IMemberCancellationService _memberCancellationService;

    public MemberSubscriptionCancellationServiceTest()
    {
      _userRoleMembershipService = new Mock<IUserRoleMembershipService>();
      _emailService = new Mock<IEmailService>();
      _userLookup = new Mock<IUserLookupService>();
      _repository = new Mock<IRepository>();
      _subscriptionPeriodCalculationsService = new Mock<IMemberSubscriptionPeriodCalculationsService>();
      _memberCancellationService = new MemberSubscriptionCancellationService(_userRoleMembershipService.Object,
        _emailService.Object, _userLookup.Object, _repository.Object, _subscriptionPeriodCalculationsService.Object);
    }
  }

}
