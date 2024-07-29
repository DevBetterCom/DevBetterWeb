using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Core.Specs;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests;

public class VerifyValidEmailAndInviteCode
{
  private readonly IRepository<Invitation> _invitationRepository = Substitute.For<IRepository<Invitation>>();
  private readonly IUserRoleMembershipService _userRoleMembershipService = Substitute.For<IUserRoleMembershipService>();
  private readonly IPaymentHandlerSubscription _paymentHandlerSubscription = Substitute.For<IPaymentHandlerSubscription>();
  private readonly IEmailService _emailService = Substitute.For<IEmailService>();
  private readonly IMemberRegistrationService _memberRegistrationService = Substitute.For<IMemberRegistrationService>();
  private readonly IAppLogger<NewMemberService> _logger = Substitute.For<IAppLogger<NewMemberService>>();

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
    _newMemberService = new NewMemberService(_invitationRepository,
      _userRoleMembershipService,
      _paymentHandlerSubscription,
      _emailService,
      _memberRegistrationService,
      _logger,
              null!); // TODO: Add dependency
  }

  [Fact]
  public async Task ReturnsSuccessGivenValidEmailAndInviteCode()
  {
    var invitation = new Invitation(_email, _inviteCode, _subscriptionId);

    _invitationRepository.FirstOrDefaultAsync(Arg.Any<InvitationByInviteCodeSpec>(), CancellationToken.None).Returns(invitation);

    var result = await _newMemberService.VerifyValidEmailAndInviteCodeAsync(_email, _inviteCode);

    await _invitationRepository.Received(1).FirstOrDefaultAsync(Arg.Any<InvitationByInviteCodeSpec>(), CancellationToken.None);
    Assert.Equal(_validEmailAndInviteCodeString, result.Value);
  }

  [Fact]
  public async Task ReturnsExceptionMessageGivenInvalidEmail()
  {
    var invitation = new Invitation("", _inviteCode, _subscriptionId);

    _invitationRepository.FirstOrDefaultAsync(Arg.Any<InvitationByInviteCodeSpec>(), CancellationToken.None).Returns(invitation);

    var result = await _newMemberService.VerifyValidEmailAndInviteCodeAsync(_email, _inviteCode);

    await _invitationRepository.Received(1).FirstOrDefaultAsync(Arg.Any<InvitationByInviteCodeSpec>(), CancellationToken.None);
    Assert.Equal(_invalidEmailString, result.Value);
  }

  [Fact]
  public async Task ReturnsExceptionMessageGivenInvalidInviteCode()
  {
    _ = _invitationRepository.FirstOrDefaultAsync(Arg.Any<InvitationByInviteCodeSpec>(), CancellationToken.None).Returns((Invitation)null!);

    var result = await _newMemberService.VerifyValidEmailAndInviteCodeAsync(_email, _inviteCode);

    await _invitationRepository.Received(1).FirstOrDefaultAsync(Arg.Any<InvitationByInviteCodeSpec>(), CancellationToken.None);
    Assert.Equal(_invalidInviteCodeString, result.Value);
  }

  [Fact]
  public async Task ReturnsExceptionMessageGivenInactiveInviteCode()
  {
    Invitation invitation = new Invitation(_email, _inviteCode, _subscriptionId);

    invitation.Deactivate();

    _invitationRepository.FirstOrDefaultAsync(Arg.Any<InvitationByInviteCodeSpec>(), CancellationToken.None).Returns(invitation);

    var result = await _newMemberService.VerifyValidEmailAndInviteCodeAsync(_email, _inviteCode);

    await _invitationRepository.Received(1).FirstOrDefaultAsync(Arg.Any<InvitationByInviteCodeSpec>(), CancellationToken.None);
    Assert.Equal(_inactiveInviteString, result.Value);
  }
}
