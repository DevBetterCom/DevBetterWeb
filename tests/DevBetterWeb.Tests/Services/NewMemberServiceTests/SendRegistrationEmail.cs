using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests;

public class SendRegistrationEmail
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
  private readonly string _inviteCode = "TestInviteCode";

  private readonly Invitation _invitation;

  public SendRegistrationEmail()
  {
    _newMemberService = new NewMemberService(_invitationRepository,
      _userRoleMembershipService,
      _paymentHandlerSubscription,
      _emailService,
      _memberRegistrationService,
      _logger,
              null!); // TODO: Add dependency

    _invitation = new Invitation(_email, _inviteCode, _subscriptionId);
  }

  [Fact]
  public async Task SendsRegistrationEmail()
  {
    await _newMemberService.SendRegistrationEmailAsync(_invitation);

    await _emailService.Received(1).SendEmailAsync(_email, Arg.Any<string>(), Arg.Any<string>());
  }

}
