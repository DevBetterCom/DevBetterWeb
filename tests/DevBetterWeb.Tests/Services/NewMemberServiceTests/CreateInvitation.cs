using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using Xunit;
using Moq;
using DevBetterWeb.Core.Entities;
using System.Threading;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests
{
  public class CreateInvitation
  {
    private readonly Mock<IRepository<Member>> _memberRepository;
    private readonly Mock<IRepository<Invitation>> _invitationRepository;
    private readonly Mock<IUserRoleMembershipService> _userRoleMembershipService;
    private readonly Mock<IPaymentHandlerSubscription> _paymentHandlerSubscription;
    private readonly Mock<IEmailService> _emailService;
    private readonly Mock<IMemberRegistrationService> _memberRegistrationService;
    private readonly Mock<IMemberSubscriptionCreationService> _memberSubscriptionCreationService;

    private readonly INewMemberService _newMemberService;

    private readonly string _email = "TestEmail";
    private readonly string _subscriptionId = "TestSubscriptionId";

    public CreateInvitation()
    {
      _invitationRepository = new Mock<IRepository<Invitation>>();
      _memberRepository = new Mock<IRepository<Member>>();
      _userRoleMembershipService = new Mock<IUserRoleMembershipService>();
      _paymentHandlerSubscription = new Mock<IPaymentHandlerSubscription>();
      _emailService = new Mock<IEmailService>();
      _memberRegistrationService = new Mock<IMemberRegistrationService>();
      _memberSubscriptionCreationService = new Mock<IMemberSubscriptionCreationService>();
      _newMemberService = new NewMemberService(_memberRepository.Object, _invitationRepository.Object, _userRoleMembershipService.Object, _paymentHandlerSubscription.Object, _emailService.Object, _memberRegistrationService.Object, _memberSubscriptionCreationService.Object);
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
}
