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
      testlist.Add(InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(2));

      var list = service.CheckIfAnyActiveInvitationsRequireUserPing(testlist);

      Assert.Equal(testlist, list);
    }

    [Fact]
    public void ReturnsEmptyListIfListContainsActiveInvitationCreated2DaysAgoWithUserPingDateOfToday()
    {
      testlist.Add(InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(2)
        .WithDateOfUserPingGivenDaysAgo(0));

      var list = service.CheckIfAnyActiveInvitationsRequireUserPing(testlist);

      Assert.Empty(list);
    }

    [Fact]
    public void ReturnsEmptyListIfListContainsActiveInvitationCreated5DaysAgoWithUserPingDateOf3DaysAgo()
    {
      testlist.Add(InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(5)
       .WithDateOfUserPingGivenDaysAgo(3));

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

    public static IEnumerable<object[]> GetInvitations()
    {
      yield return new object[]
      {
        InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(3),
        InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(5)
        .WithDateOfUserPingGivenDaysAgo(3)
      };
      yield return new object[]
      {
        InvitationBuilder.BuildDefaultInvitation()
        .WithDateCreatedGivenDaysAgo(3),
        InvitationBuilder.BuildDefaultInvitation()
        .AndDeactivated()
      };
    }
  }
}
