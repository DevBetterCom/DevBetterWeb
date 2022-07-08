using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Core.Specs;
using Moq;
using Xunit;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests;

public class MemberSetup
{
  private readonly Mock<IRepository<Member>> _memberRepository = new();
  private readonly Mock<IRepository<Invitation>> _invitationRepository = new();
  private readonly Mock<IUserRoleMembershipService> _userRoleMembershipService = new();
  private readonly Mock<IPaymentHandlerSubscription> _paymentHandlerSubscription = new();
  private readonly Mock<IEmailService> _emailService = new();
  private readonly Mock<IMemberRegistrationService> _memberRegistrationService = new();
  private readonly Mock<IAppLogger<NewMemberService>> _logger = new();
  private readonly Mock<IMemberAddBillingActivityService> _memberAddBillingActivityService = new();

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
    _newMemberService = new NewMemberService(_invitationRepository.Object,
      _userRoleMembershipService.Object,
      _paymentHandlerSubscription.Object,
      _emailService.Object,
      _memberRegistrationService.Object,
      _logger.Object,
      _memberAddBillingActivityService.Object);
    _invitation = new Invitation(_email, _inviteCode, _subscriptionId);
  }

  [Fact]
  public async Task SetsUpNewMember()
  {
    var memberResult = new Member();
    var memberId = memberResult.Id;

    _invitationRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<InvitationByInviteCodeSpec>(), CancellationToken.None)).ReturnsAsync(_invitation);
    _invitationRepository.Setup(r => r.ListAsync(It.IsAny<ActiveInvitationByEmailSpec>(), CancellationToken.None)).ReturnsAsync(new List<Invitation>());
    _memberRepository.Setup(r => r.GetByIdAsync(memberId, CancellationToken.None)).ReturnsAsync(memberResult);
    _memberRegistrationService.Setup(r => r.RegisterMemberAsync(_userId)).ReturnsAsync(memberResult);

    Member member = await _newMemberService.MemberSetupAsync(_userId, _firstName, _lastName, _inviteCode, "");

    Assert.Equal(_firstName, member.FirstName);
    Assert.Equal(_lastName, member.LastName);

    _userRoleMembershipService.Verify(u => u.AddUserToRoleByRoleNameAsync(_userId, _roleName), Times.Once);
    Assert.False(_invitation.Active);
  }
}
