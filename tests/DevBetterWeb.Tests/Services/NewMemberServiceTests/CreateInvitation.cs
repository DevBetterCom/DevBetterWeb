using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests;

public class CreateInvitation
{
  private readonly IRepository<Invitation> _invitationRepository = Substitute.For<IRepository<Invitation>>();
  private readonly IUserRoleMembershipService _userRoleMembershipService = Substitute.For<IUserRoleMembershipService>();
  private readonly IPaymentHandlerSubscription _paymentHandlerSubscription = Substitute.For<IPaymentHandlerSubscription>();
  private readonly IEmailService _emailService = Substitute.For<IEmailService>();
  private readonly IMemberRegistrationService _memberRegistrationService = Substitute.For<IMemberRegistrationService>();
  private readonly IAppLogger<NewMemberService> _logger = Substitute.For<IAppLogger<NewMemberService>>();

  private readonly INewMemberService _newMemberService;

  private readonly string _email = "TestEmail";
  private readonly string _subscriptionId = "TestSubscriptionId";

  public CreateInvitation()
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
  public async Task CreatesInvitationWithGivenEmailAndEventId()
  {
    var invitation = await _newMemberService.CreateInvitationAsync(_email, _subscriptionId);

    Assert.Equal(_email, invitation.Email);
    Assert.Equal(_subscriptionId, invitation.PaymentHandlerSubscriptionId);

    await _invitationRepository.Received(1).AddAsync(invitation, CancellationToken.None);
  }
}
