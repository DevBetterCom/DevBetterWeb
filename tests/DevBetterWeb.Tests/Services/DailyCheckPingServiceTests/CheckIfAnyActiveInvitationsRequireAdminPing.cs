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

    private List<Invitation> testlist;

    public CheckIfAnyActiveInvitationsRequireAdminPing()
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
      var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

      Assert.Empty(list);
    }

    [Fact]
    public void ReturnsListGiven4DayOldInvitationWithUserPingDateAndNoAdminPingDate()
    {
      testlist.Add(InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2));

      var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

      Assert.Contains(testlist[0], list);
    }

    [Fact]
    public void ReturnsListGiven4DayOldInvitationWithUserPingDateAndAdminPingDateOneDayAgo()
    {
      testlist.Add(InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(1));

      var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

      Assert.Contains(testlist[0], list);
    }

    [Fact]
    public void ReturnsEmptyListGivenInvitationWithNoUserPingDate()
    {
      testlist.Add(InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(4));

      var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

      Assert.Empty(list);
    }

    [Fact]
    public void ReturnsEmptyListGivenInvitationWithAdminPingToday()
    {
      testlist.Add(InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(0));

      var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

      Assert.Empty(list);

    }

    [Fact]
    public void ReturnsEmptyListGivenInvitationCreated3DaysAgoToday()
    {
      testlist.Add(InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(3)
        .WithDateOfUserPingGivenDaysAgo(1));

      var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

      Assert.Empty(list);

    }

    [Fact]
    public void ReturnsEmptyListGivenInvitationWithUserPingYesterday()
    {
      testlist.Add(InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(1));

      var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

      Assert.Empty(list);

    }

    [Fact]
    public void ReturnsEmptyListGivenInactiveInvitation()
    {
      testlist.Add(InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(3)
        .WithDateOfLastAdminPingGivenDaysAgo(1)
        .AndDeactivated());

      var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

      Assert.Empty(list);

    }

    [Theory]
    [MemberData(nameof(GetInvitations))]
    public void ReturnsOnlyValidAdminPingInviteGivenValidAndInvalid(Invitation valid, Invitation invalid)
    {
      testlist.Add(valid);
      testlist.Add(invalid);

      var list = service.CheckIfAnyActiveInvitationsRequireAdminPing(testlist);

      Assert.Contains(valid, list);
      Assert.DoesNotContain(invalid, list);
    }

    public static IEnumerable<object[]> GetInvitations()
    {
      yield return new object[]
      {
        InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(1),
        InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(1)
        .AndDeactivated()
      };
      yield return new object[]
      {
        InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(5)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(1),
        InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(5)
        .WithDateOfUserPingGivenDaysAgo(1)
      };
      yield return new object[]
      {
        InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(5)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(1),
        InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(3)
        .WithDateOfUserPingGivenDaysAgo(2)
      };
      yield return new object[]
      {
        InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(7)
        .WithDateOfUserPingGivenDaysAgo(4)
        .WithDateOfLastAdminPingGivenDaysAgo(2),
        InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(0)
      };
    }
  }
}
