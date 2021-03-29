using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using Xunit;
using Moq;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Specs;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests
{
  public class MemberSetup
  {
    private readonly Mock<IRepository> _repository;
    private readonly Mock<IUserRoleMembershipService> _userRoleMembershipService;
    private readonly Mock<IPaymentHandlerSubscription> _paymentHandlerSubscription;
    private readonly Mock<IEmailService> _emailService;
    private readonly Mock<IMemberRegistrationService> _memberRegistrationService;

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
      _repository = new Mock<IRepository>();
      _userRoleMembershipService = new Mock<IUserRoleMembershipService>();
      _paymentHandlerSubscription = new Mock<IPaymentHandlerSubscription>();
      _emailService = new Mock<IEmailService>();
      _memberRegistrationService = new Mock<IMemberRegistrationService>();
      _newMemberService = new NewMemberService(_repository.Object, _userRoleMembershipService.Object, _paymentHandlerSubscription.Object, _emailService.Object, _memberRegistrationService.Object);
      _invitation = new Invitation(_email, _inviteCode, _subscriptionId);
    }

    [Fact]
    public async Task SetsUpNewMember()
    {
      var memberResult = new Member();

      _repository.Setup(r => r.GetAsync(It.IsAny<InvitationByInviteCodeSpec>())).ReturnsAsync(_invitation);
      _memberRegistrationService.Setup(r => r.RegisterMemberAsync(_userId)).ReturnsAsync(memberResult);

      Member member = await _newMemberService.MemberSetupAsync(_userId, _firstName, _lastName, _inviteCode);

      Assert.Equal(_firstName, member.FirstName);
      Assert.Equal(_lastName, member.LastName);

      _userRoleMembershipService.Verify(u => u.AddUserToRoleByRoleNameAsync(_userId, _roleName), Times.Once);
      Assert.False(_invitation.Active);

    }

  }

}
