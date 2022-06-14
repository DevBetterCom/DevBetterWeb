using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Core.Specs;
using Moq;
using Xunit;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests;

public class VerifyValidEmailAndInviteCode
{
  private readonly Mock<IRepository<Member>> _memberRepository = new();
  private readonly Mock<IRepository<Invitation>> _invitationRepository = new();
  private readonly Mock<IUserRoleMembershipService> _userRoleMembershipService = new();
  private readonly Mock<IPaymentHandlerSubscription> _paymentHandlerSubscription = new();
  private readonly Mock<IEmailService> _emailService = new();
  private readonly Mock<IMemberRegistrationService> _memberRegistrationService = new();
  private readonly Mock<IAppLogger<NewMemberService>> _logger = new();

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
    _newMemberService = new NewMemberService(_invitationRepository.Object,
      _userRoleMembershipService.Object,
      _paymentHandlerSubscription.Object,
      _emailService.Object,
      _memberRegistrationService.Object,
      _logger.Object,
              null!); // TODO: Add dependency

  }

  [Fact]
  public async Task ReturnsSuccessGivenValidEmailAndInviteCode()
  {
    var invitation = new Invitation(_email, _inviteCode, _subscriptionId);

    _invitationRepository.Setup(r => r.GetBySpecAsync(It.IsAny<InvitationByInviteCodeSpec>(), CancellationToken.None)).ReturnsAsync(invitation);

    var result = await _newMemberService.VerifyValidEmailAndInviteCodeAsync(_email, _inviteCode);

    _invitationRepository.Verify(r => r.GetBySpecAsync(It.IsAny<InvitationByInviteCodeSpec>(), CancellationToken.None), Times.Once);
    Assert.Equal(_validEmailAndInviteCodeString, result.Value);
  }

  [Fact]
  public async Task ReturnsExceptionMessageGivenInvalidEmail()
  {
    var invitation = new Invitation("", _inviteCode, _subscriptionId);

    _invitationRepository.Setup(r => r.GetBySpecAsync(It.IsAny<InvitationByInviteCodeSpec>(), CancellationToken.None)).ReturnsAsync(invitation);

    var result = await _newMemberService.VerifyValidEmailAndInviteCodeAsync(_email, _inviteCode);

    _invitationRepository.Verify(r => r.GetBySpecAsync(It.IsAny<InvitationByInviteCodeSpec>(), CancellationToken.None), Times.Once);
    Assert.Equal(_invalidEmailString, result.Value);
  }

  [Fact]
  public async Task ReturnsExceptionMessageGivenInvalidInviteCode()
  {

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    _ = _invitationRepository.Setup(r => r.GetBySpecAsync(It.IsAny<InvitationByInviteCodeSpec>(), CancellationToken.None)).ReturnsAsync((Invitation)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

    var result = await _newMemberService.VerifyValidEmailAndInviteCodeAsync(_email, _inviteCode);

    _invitationRepository.Verify(r => r.GetBySpecAsync(It.IsAny<InvitationByInviteCodeSpec>(), CancellationToken.None), Times.Once);
    Assert.Equal(_invalidInviteCodeString, result.Value);
  }

  [Fact]
  public async Task ReturnsExceptionMessageGivenInactiveInviteCode()
  {
    Invitation _invitation = new Invitation(_email, _inviteCode, _subscriptionId);

    _invitation.Deactivate();

    _invitationRepository.Setup(r => r.GetBySpecAsync(It.IsAny<InvitationByInviteCodeSpec>(), CancellationToken.None)).ReturnsAsync(_invitation);

    var result = await _newMemberService.VerifyValidEmailAndInviteCodeAsync(_email, _inviteCode);

    _invitationRepository.Verify(r => r.GetBySpecAsync(It.IsAny<InvitationByInviteCodeSpec>(), CancellationToken.None), Times.Once);
    Assert.Equal(_inactiveInviteString, result.Value);
  }
}
