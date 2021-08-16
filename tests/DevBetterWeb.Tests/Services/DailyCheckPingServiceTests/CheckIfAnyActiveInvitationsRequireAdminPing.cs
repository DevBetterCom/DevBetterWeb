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
    private readonly Mock<IRepository<Invitation>> _repository = new();
    private readonly Mock<IRepository<Member>> _memberRepository = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<UserManager<ApplicationUser>> _userManager;

    private DailyCheckPingService _service;

    private List<Invitation> _testlist = new();
    private InvitationBuilder _builder = new();

    public CheckIfAnyActiveInvitationsRequireAdminPing()
    {
      var store = new Mock<IUserStore<ApplicationUser>>();
      _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
      _service = new DailyCheckPingService(_repository.Object, _memberRepository.Object, _emailService.Object, _userManager.Object);
    }

    [Fact]
    public void ReturnsEmptyListGivenEmptyList()
    {
      var list = _service.CheckIfAnyActiveInvitationsRequireAdminPing(_testlist);

      Assert.Empty(list);
    }

    [Fact]
    public void ReturnsListGiven4DayOldInvitationWithUserPingDateAndNoAdminPingDate()
    {
      _testlist.Add(_builder.WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2)
        .Build());

      var list = _service.CheckIfAnyActiveInvitationsRequireAdminPing(_testlist);

      Assert.Contains(_testlist[0], list);
    }

    [Fact]
    public void ReturnsListGiven4DayOldInvitationWithUserPingDateAndAdminPingDateOneDayAgo()
    {
      _testlist.Add(_builder.WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(1)
        .Build());

      var list = _service.CheckIfAnyActiveInvitationsRequireAdminPing(_testlist);

      Assert.Contains(_testlist[0], list);
    }

    //[Fact]
    // TODO: Diagnose and fix
    //public void ReturnsEmptyListGivenInvitationWithNoUserPingDate()
    //{
    //  _testlist.Add(_builder.WithDateCreatedGivenDaysAgo(4)
    //    .Build());

    //  var list = _service.CheckIfAnyActiveInvitationsRequireAdminPing(_testlist);

    //  Assert.Empty(list);
    //}

    [Fact]
    public void ReturnsEmptyListGivenInvitationWithAdminPingToday()
    {
      _testlist.Add(_builder.WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(0)
        .Build());

      var list = _service.CheckIfAnyActiveInvitationsRequireAdminPing(_testlist);

      Assert.Empty(list);

    }

    [Fact]
    public void ReturnsEmptyListGivenInvitationCreated3DaysAgoToday()
    {
      _testlist.Add(_builder.WithDateCreatedGivenDaysAgo(3)
        .WithDateOfUserPingGivenDaysAgo(1)
        .Build());

      var list = _service.CheckIfAnyActiveInvitationsRequireAdminPing(_testlist);

      Assert.Empty(list);

    }

    [Fact]
    public void ReturnsEmptyListGivenInvitationWithUserPingYesterday()
    {
      _testlist.Add(_builder.WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(1)
        .Build());

      var list = _service.CheckIfAnyActiveInvitationsRequireAdminPing(_testlist);

      Assert.Empty(list);

    }

    [Fact]
    public void ReturnsEmptyListGivenInactiveInvitation()
    {
      _testlist.Add(_builder.WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(3)
        .WithDateOfLastAdminPingGivenDaysAgo(1)
        .AndDeactivated()
        .Build());

      var list = _service.CheckIfAnyActiveInvitationsRequireAdminPing(_testlist);

      Assert.Empty(list);

    }

    [Theory]
    [MemberData(nameof(GetInvitations))]
    public void ReturnsOnlyValidAdminPingInviteGivenValidAndInvalid(Invitation valid, Invitation invalid)
    {
      _testlist.Add(valid);
      _testlist.Add(invalid);

      var list = _service.CheckIfAnyActiveInvitationsRequireAdminPing(_testlist);

      Assert.Contains(valid, list);
      Assert.DoesNotContain(invalid, list);
    }

    public static IEnumerable<object[]> GetInvitations()
    {
      yield return new object[]
      {
        new InvitationBuilder().WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(1)
        .Build(),
        new InvitationBuilder().WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(1)
        .AndDeactivated()
        .Build()
      };

      yield return new object[]
      {
        new InvitationBuilder().WithDateCreatedGivenDaysAgo(5)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(1)
        .Build(),
        new InvitationBuilder().WithDateCreatedGivenDaysAgo(5)
        .WithDateOfUserPingGivenDaysAgo(1)
        .Build()
      };
      yield return new object[]
      {
        new InvitationBuilder().WithDateCreatedGivenDaysAgo(5)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(1)
        .Build(),
        new InvitationBuilder().WithDateCreatedGivenDaysAgo(3)
        .WithDateOfUserPingGivenDaysAgo(2)
        .Build()
      };
      yield return new object[]
      {
        new InvitationBuilder().WithDateCreatedGivenDaysAgo(7)
        .WithDateOfUserPingGivenDaysAgo(4)
        .WithDateOfLastAdminPingGivenDaysAgo(2)
        .Build(),
        new InvitationBuilder().WithDateCreatedGivenDaysAgo(4)
        .WithDateOfUserPingGivenDaysAgo(2)
        .WithDateOfLastAdminPingGivenDaysAgo(0)
        .Build()
      };
    }
  }
}
