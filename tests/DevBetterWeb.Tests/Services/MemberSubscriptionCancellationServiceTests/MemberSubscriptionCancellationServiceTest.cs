using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using NSubstitute;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionCancellationServiceTests;

public class MemberSubscriptionCancellationServiceTest
{
  internal readonly IUserRoleMembershipService _userRoleMembershipService;
  internal readonly IEmailService _emailService;
  internal readonly IUserLookupService _userLookup;
  internal readonly IMemberLookupService _memberLookup;
  internal readonly IRepository<Member> _memberRepository;
  internal readonly IMemberSubscriptionPeriodCalculationsService _subscriptionPeriodCalculationsService;

  internal readonly IMemberCancellationService _memberCancellationService;

  public MemberSubscriptionCancellationServiceTest()
  {
    _userRoleMembershipService = Substitute.For<IUserRoleMembershipService>();
    _emailService = Substitute.For<IEmailService>();
    _userLookup = Substitute.For<IUserLookupService>();
    _memberLookup = Substitute.For<IMemberLookupService>();
    _memberRepository = Substitute.For<IRepository<Member>>();
    _subscriptionPeriodCalculationsService = Substitute.For<IMemberSubscriptionPeriodCalculationsService>();
    _memberCancellationService = new MemberSubscriptionCancellationService(_userRoleMembershipService,
      _emailService, _userLookup, _memberLookup, _memberRepository, _subscriptionPeriodCalculationsService);
  }
}
