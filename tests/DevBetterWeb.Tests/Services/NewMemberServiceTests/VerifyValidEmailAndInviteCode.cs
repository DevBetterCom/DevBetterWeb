using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using Xunit;
using Moq;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Specs;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests
{
  public class VerifyValidEmailAndInviteCode
  {
    private readonly Mock<IRepository> _repository;
    private readonly Mock<IUserRoleMembershipService> _userRoleMembershipService;
    private readonly Mock<IPaymentHandlerSubscription> _paymentHandlerSubscription;
    private readonly Mock<IEmailService> _emailService;

    private readonly INewMemberService _newMemberService;

    private readonly string _email = "TestEmail";
    private readonly string _inviteCode = "TestInviteCode";
    private readonly string _subscriptionId = "TestSubscriptionId";

    private readonly string _validEmailAndInviteCodeString = "success";
    private readonly string _invalidEmailString = "Invalid email or invite code: DevBetterWeb.Core.Exceptions.InvalidEmailException";
    private readonly string _invalidInviteCodeString = "Invalid email or invite code: DevBetterWeb.Core.Exceptions.InvitationNotFoundException";
    private readonly string _inactiveInviteString = "Invalid email or invite code: DevBetterWeb.Core.Exceptions.InvitationNotActiveException";

    public VerifyValidEmailAndInviteCode()
    {
      _repository = new Mock<IRepository>();
      _userRoleMembershipService = new Mock<IUserRoleMembershipService>();
      _paymentHandlerSubscription = new Mock<IPaymentHandlerSubscription>();
      _emailService = new Mock<IEmailService>();
      _newMemberService = new NewMemberService(_repository.Object, _userRoleMembershipService.Object, _paymentHandlerSubscription.Object, _emailService.Object);
    }

    [Fact]
    public async Task ReturnsSuccessGivenValidEmailAndInviteCode()
    {
      Invitation _invitation = new Invitation(_email, _inviteCode, _subscriptionId);

      _repository.Setup(r => r.GetAsync(It.IsAny<InvitationByInviteCodeWithEmailSpec>())).ReturnsAsync(_invitation);

      var result = await _newMemberService.VerifyValidEmailAndInviteCode(_email, _inviteCode);

      _repository.Verify(r => r.GetAsync(It.IsAny<InvitationByInviteCodeWithEmailSpec>()), Times.Once);
      Assert.Equal(_validEmailAndInviteCodeString, result);
    }

    [Fact]
    public async Task ReturnsExceptionMessageGivenInvalidEmail()
    {
      Invitation _invitation = new Invitation(null, _inviteCode, _subscriptionId);

      _repository.Setup(r => r.GetAsync(It.IsAny<InvitationByInviteCodeWithEmailSpec>())).ReturnsAsync(_invitation);

      var result = await _newMemberService.VerifyValidEmailAndInviteCode(_email, _inviteCode);

      _repository.Verify(r => r.GetAsync(It.IsAny<InvitationByInviteCodeWithEmailSpec>()), Times.Once);
      Assert.Equal(_invalidEmailString, result);
    }

    [Fact]
    public async Task ReturnsExceptionMessageGivenInvalidInviteCode()
    {

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
      _ = _repository.Setup(r => r.GetAsync(It.IsAny<InvitationByInviteCodeWithEmailSpec>())).ReturnsAsync((Invitation)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

      var result = await _newMemberService.VerifyValidEmailAndInviteCode(_email, _inviteCode);

      _repository.Verify(r => r.GetAsync(It.IsAny<InvitationByInviteCodeWithEmailSpec>()), Times.Once);
      Assert.Equal(_invalidInviteCodeString, result);
    }

    [Fact]
    public async Task ReturnsExceptionMessageGivenInactiveInviteCode()
    {
      Invitation _invitation = new Invitation(_email, _inviteCode, _subscriptionId);

      _invitation.Deactivate();

      _repository.Setup(r => r.GetAsync(It.IsAny<InvitationByInviteCodeWithEmailSpec>())).ReturnsAsync(_invitation);

      var result = await _newMemberService.VerifyValidEmailAndInviteCode(_email, _inviteCode);

      _repository.Verify(r => r.GetAsync(It.IsAny<InvitationByInviteCodeWithEmailSpec>()), Times.Once);
      Assert.Equal(_inactiveInviteString, result);
    }

  }

}
