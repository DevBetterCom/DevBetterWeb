using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using Xunit;
using Moq;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests
{
  public class SendRegistrationEmail
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
    private readonly string _inviteCode = "TestInviteCode";

    private readonly Invitation _invitation;

    public SendRegistrationEmail()
    {
      _newMemberService = new NewMemberService(_invitationRepository.Object,
        _userRoleMembershipService.Object,
        _paymentHandlerSubscription.Object,
        _emailService.Object,
        _memberRegistrationService.Object,
        _logger.Object,
                null); // TODO: Add dependency

      _invitation = new Invitation(_email, _inviteCode, _subscriptionId);
    }

    [Fact]
    public async Task SendsRegistrationEmail()
    {
      await _newMemberService.SendRegistrationEmailAsync(_invitation);

      _emailService.Verify(e => e.SendEmailAsync(_email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

  }

}
