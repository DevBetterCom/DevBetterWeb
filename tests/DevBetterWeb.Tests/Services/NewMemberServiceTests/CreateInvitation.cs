using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using Moq;
using Xunit;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests;

public class CreateInvitation
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
  private readonly string _subscriptionId = "TestSubscriptionId";

  public CreateInvitation()
  {
    _newMemberService = new NewMemberService(_invitationRepository.Object,
    _userRoleMembershipService.Object,
    _paymentHandlerSubscription.Object,
    _emailService.Object,
    _memberRegistrationService.Object,
    _logger.Object,
            null); // TODO: Add dependency

  }

  [Fact]
  public async Task CreatesInvitationWithGivenEmailAndEventId()
  {
    var invitation = await _newMemberService.CreateInvitationAsync(_email, _subscriptionId);

    Assert.Equal(_email, invitation.Email);
    Assert.Equal(_subscriptionId, invitation.PaymentHandlerSubscriptionId);

    _invitationRepository.Verify(r => r.AddAsync(invitation, CancellationToken.None), Times.Once);
  }
}
