using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Core.Specs;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests;

public class MemberSetup
{
  private readonly IRepository<Member> _memberRepository = Substitute.For<IRepository<Member>>();
  private readonly IRepository<Invitation> _invitationRepository = Substitute.For<IRepository<Invitation>>();
  private readonly IUserRoleMembershipService _userRoleMembershipService = Substitute.For<IUserRoleMembershipService>();
  private readonly IPaymentHandlerSubscription _paymentHandlerSubscription = Substitute.For<IPaymentHandlerSubscription>();
  private readonly IEmailService _emailService = Substitute.For<IEmailService>();
  private readonly IMemberRegistrationService _memberRegistrationService = Substitute.For<IMemberRegistrationService>();
  private readonly IAppLogger<NewMemberService> _logger = Substitute.For<IAppLogger<NewMemberService>>();
  private readonly IMemberAddBillingActivityService _memberAddBillingActivityService = Substitute.For<IMemberAddBillingActivityService>();

  private readonly INewMemberService _newMemberService;

  private readonly string _userId = "TestUserId";
  private readonly string _firstName = "TestFirstName";
  private readonly string _lastName = "TestLastName";
  private readonly string _inviteCode = "TestInviteCode";

  private readonly string _email = "TestEmail";
  private readonly string _subscriptionId = "TestSubscriptionId";
  private readonly Invitation _invitation;

  private readonly string _roleName = "Members";

  public MemberSetup()
  {
    _newMemberService = new NewMemberService(_invitationRepository,
      _userRoleMembershipService,
      _paymentHandlerSubscription,
      _emailService,
      _memberRegistrationService,
      _logger,
      _memberAddBillingActivityService);
    _invitation = new Invitation(_email, _inviteCode, _subscriptionId);
  }

  [Fact]
  public async Task SetsUpNewMember()
  {
    var memberResult = new Member();
    var memberId = memberResult.Id;

    _invitationRepository.FirstOrDefaultAsync(Arg.Any<InvitationByInviteCodeSpec>(), CancellationToken.None).Returns(_invitation);
    _invitationRepository.ListAsync(Arg.Any<ActiveInvitationByEmailSpec>(), CancellationToken.None).Returns(new List<Invitation>());
    _memberRepository.GetByIdAsync(memberId, CancellationToken.None).Returns(memberResult);
    _memberRegistrationService.RegisterMemberAsync(_userId).Returns(memberResult);

    Member member = await _newMemberService.MemberSetupAsync(_userId, _firstName, _lastName, _inviteCode, "");

    Assert.Equal(_firstName, member.FirstName);
    Assert.Equal(_lastName, member.LastName);

    await _userRoleMembershipService.Received(1).AddUserToRoleByRoleNameAsync(_userId, _roleName);
    Assert.False(_invitation.Active);
  }
}
