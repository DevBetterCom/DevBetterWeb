using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DevBetterWeb.Tests.Services.MemberSubscriptionEndedAdminEmailServiceTests;

public class SendMemberSubscriptionEndedEmailAsync
{
  private readonly MemberSubscriptionEndedAdminEmailService _memberSubscriptionEndedAdminEmailService;

  private readonly Mock<UserManager<ApplicationUser>> _userManager;
  private readonly Mock<IEmailService> _emailService;
  private readonly Mock<IMemberLookupService> _memberLookup;

  public SendMemberSubscriptionEndedEmailAsync()
  {
    var store = new Mock<IUserStore<ApplicationUser>>();
    _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
    _emailService = new Mock<IEmailService>();
    _memberLookup = new Mock<IMemberLookupService>();
    _memberSubscriptionEndedAdminEmailService = new MemberSubscriptionEndedAdminEmailService(
      _userManager.Object, _emailService.Object, _memberLookup.Object);
  }

  [Fact]
  public async Task SendsEmail()
  {
    _memberLookup.Setup(m => m.GetMemberByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Mock<Member>().Object);
    _userManager.Setup(u => u.GetUsersInRoleAsync(AuthConstants.Roles.ADMINISTRATORS)).ReturnsAsync(
      new List<ApplicationUser> { new Mock<ApplicationUser>().Object, new Mock<ApplicationUser>().Object });

    string testEmail = "TestEmail";
    await _memberSubscriptionEndedAdminEmailService.SendMemberSubscriptionEndedEmailAsync(testEmail);

    _emailService.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
  }
}
