using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using Xunit;
using Moq;
using Ardalis.Specification;

namespace DevBetterWeb.Tests.Services.NewMemberServiceTests
{

  public class CreateInvitation
  {
    private readonly Mock<IRepository> _repository;
    private readonly Mock<IUserRoleMembershipService> _userRoleMembershipService;
    private readonly Mock<IPaymentHandlerSubscription> _paymentHandlerSubscription;
    private readonly Mock<IEmailService> _emailService;

    private readonly INewMemberService _newMemberService;

    private readonly string _email = "TestEmail";
    private readonly string _subscriptionId = "TestSubscriptionId";

    public CreateInvitation()
    {
      _repository = new Mock<IRepository>();
      _userRoleMembershipService = new Mock<IUserRoleMembershipService>();
      _paymentHandlerSubscription = new Mock<IPaymentHandlerSubscription>();
      _emailService = new Mock<IEmailService>();
      _newMemberService = new NewMemberService(_repository.Object, _userRoleMembershipService.Object, _paymentHandlerSubscription.Object, _emailService.Object);
    }

    [Fact]
    public async Task CreatesInvitationWithGivenEmailAndEventId()
    {
      var invitation = await _newMemberService.CreateInvitation(_email, _subscriptionId);

      Assert.Equal(_email, invitation.Email);
      Assert.Equal(_subscriptionId, invitation.PaymentHandlerSubscriptionId);

      _repository.Verify(r => r.AddAsync(invitation), Times.Once);

    }
  }

}
