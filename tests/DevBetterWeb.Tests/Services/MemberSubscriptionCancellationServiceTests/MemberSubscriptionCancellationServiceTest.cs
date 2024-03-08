using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using NSubstitute;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionCancellationServiceTests;

public class MemberSubscriptionCancellationServiceTest
{
  protected readonly IUserRoleMembershipService _userRoleMembershipService;
  protected readonly IEmailService _emailService;
  protected readonly IUserLookupService _userLookup;
  protected readonly IMemberLookupService _memberLookup;
  protected readonly IRepository<Member> _memberRepository;
  protected readonly IMemberSubscriptionPeriodCalculationsService _subscriptionPeriodCalculationsService;

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
