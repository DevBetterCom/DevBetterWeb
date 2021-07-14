using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DevBetterWeb.Tests.Services.DailyCheckPingServiceTests
{

  public class CheckIfAnyActiveInvitationsRequireUserPing
  {
    private readonly Mock<IRepository<Invitation>> _repository;
    private readonly Mock<IEmailService> _emailService;
    private readonly Mock<UserManager<ApplicationUser>> _userManager;

    private DailyCheckPingService service;

    private List<Invitation> testlist;

    public CheckIfAnyActiveInvitationsRequireUserPing()
    {
      var store = new Mock<IUserStore<ApplicationUser>>();
      _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
      _emailService = new Mock<IEmailService>();
      _repository = new Mock<IRepository<Invitation>>();
      service = new DailyCheckPingService(_repository.Object, _emailService.Object, _userManager.Object);

      testlist = new List<Invitation>();
    }

    [Fact]
    public void ReturnsEmptyListGivenEmptyList()
    {
      var list = service.CheckIfAnyActiveInvitationsRequireUserPing(testlist);

      Assert.Empty(list);
    }

    [Fact]
    public void ReturnsListIfListContainsActiveInvitationCreated2DaysAgoWithNoUserPingDate()
    {
      testlist.Add(ActiveInvitationCreatedGivenDaysAgoWithGivenUserPingDate(2, DateTime.MinValue));

      var list = service.CheckIfAnyActiveInvitationsRequireUserPing(testlist);

      Assert.Equal(testlist, list);
    }

    [Fact]
    public void ReturnsEmptyListIfListContainsActiveInvitationCreated2DaysAgoWithUserPingDateOfToday()
    {
      testlist.Add(ActiveInvitationCreatedGivenDaysAgoWithGivenUserPingDate(2, DateTime.Today));

      var list = service.CheckIfAnyActiveInvitationsRequireUserPing(testlist);

      Assert.Empty(list);
    }

    [Fact]
    public void ReturnsEmptyListIfListContainsActiveInvitationCreated5DaysAgoWithUserPingDateOf3DaysAgo()
    {
      testlist.Add(ActiveInvitationCreatedGivenDaysAgoWithGivenUserPingDate(5, DateTime.Today.AddDays(3)));

      var list = service.CheckIfAnyActiveInvitationsRequireUserPing(testlist);

      Assert.Empty(list);
    }

    [Theory]
    [MemberData(nameof(GetInvitations))]
    public void ReturnsListWithOneInviteGivenListWithOneValidAndOneInvalid(Invitation valid, Invitation invalid)
    {
      testlist.Add(valid);
      testlist.Add(invalid);

      var list = service.CheckIfAnyActiveInvitationsRequireUserPing(testlist);

      Assert.Contains(valid, list);
      Assert.DoesNotContain(invalid, list);
    }

    private static Invitation ActiveInvitationCreatedGivenDaysAgoWithGivenUserPingDate(int daysAgo, DateTime userPingDate)
    {
      var invite = new Invitation("", "", "", DateTime.Today.AddDays(daysAgo * -1), userPingDate, DateTime.MinValue);
      return invite;
    }

    private static Invitation InactiveInvitation()
    {
      var invite = new Invitation("", "", "");
      invite.Deactivate();
      return invite;
    }

    public static IEnumerable<object[]> GetInvitations()
    {
      yield return new object[] { ActiveInvitationCreatedGivenDaysAgoWithGivenUserPingDate(3, DateTime.MinValue), 
      ActiveInvitationCreatedGivenDaysAgoWithGivenUserPingDate(5, DateTime.Today.AddDays(-3)) };

      yield return new object[] { ActiveInvitationCreatedGivenDaysAgoWithGivenUserPingDate(3, DateTime.MinValue), InactiveInvitation() };
    }

  }
}
