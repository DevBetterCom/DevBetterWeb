using System;
using System.Collections.Generic;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DevBetterWeb.Tests.Services.DailyCheckPingServiceTests
{
  public class CheckIfAnyActiveInvitationsRequireAdminPing
  {
    private readonly Mock<IRepository<Invitation>> _repository;
    private readonly Mock<IEmailService> _emailService;
    private readonly Mock<UserManager<ApplicationUser>> _userManager;

    private DailyCheckPingService service;

    public CheckIfAnyActiveInvitationsRequireAdminPing()
    {
      var store = new Mock<IUserStore<ApplicationUser>>();
      _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
      _emailService = new Mock<IEmailService>();
      _repository = new Mock<IRepository<Invitation>>();
      service = new DailyCheckPingService(_repository.Object, _emailService.Object, _userManager.Object);
    }

    [Fact]
    public void ReturnsEmptyListGivenEmptyList()
    {
      var testlist = new List<Invitation>();

      var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

      Assert.Empty(list);
    }

    [Fact]
    public void ReturnsListGiven4DayOldInvitationWithUserPingDateAndNoAdminPingDate()
    {
      var testlist = new List<Invitation>();
      testlist.Add(InvitationWithGivenProperties(4, 2, null));

      var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

      Assert.Contains(testlist[0], list);
    }

    //[Fact]
    //public void ReturnsListGiven4DayOldInvitationWithUserPingDateAndAdminPingDateOneDayAgo()
    //{
    //  var testlist = new List<Invitation>();
    //  testlist.Add(InvitationWithGivenProperties(4, 2, 1));

    //  var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

    //  Assert.Contains(testlist[0], list);
    //}

    //[Fact]
    //public void ReturnsEmptyListGivenInvitationWithNoUserPingDate()
    //{
    //  var testlist = new List<Invitation>();
    //  testlist.Add(InvitationWithGivenProperties(4, null, null));

    //  var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

    //  Assert.Empty(list);
    //}

    //[Fact]
    //public void ReturnsEmptyListGivenInvitationWithAdminPingToday()
    //{
    //  var testlist = new List<Invitation>();
    //  testlist.Add(InvitationWithGivenProperties(4, 2, 0));

    //  var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

    //  Assert.Empty(list);

    //}

    //[Fact]
    //public void ReturnsEmptyListGivenInactiveInvitation()
    //{
    //  var testlist = new List<Invitation>();
    //  testlist.Add(InvitationWithGivenProperties(4, 3, 1, false));

    //  var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

    //  Assert.Empty(list);

    //}

    //[Theory]
    //[MemberData(nameof(GetInvitations))]
    //public void ReturnsOnlyValidAdminPingInviteGivenValidAndInvalid(Invitation valid, Invitation invalid)
    //{

    //}

    //public static IEnumerable<object[]> GetInvitations()
    //{
    //  yield return new object[] {  };

    //}


    private static Invitation InvitationWithGivenProperties(int daysOld, int? daysSinceUserPing, int? daysSinceAdminPing, bool active = true)
    {
      DateTime dateCreated = DateTime.Today.AddDays(daysOld * -1);
      DateTime userPingDate = DateTime.MinValue;
      if(daysSinceUserPing != null) DateTime.Today.AddDays((double)(daysSinceUserPing * -1));
      DateTime adminPingDate = DateTime.MinValue;
      if(daysSinceAdminPing != null) DateTime.Today.AddDays((double)(daysSinceAdminPing * -1));

      var invite = new Invitation("", "", "", dateCreated, userPingDate, adminPingDate);

      if (!active) invite.Deactivate();

      return invite;
    }
  }
}
