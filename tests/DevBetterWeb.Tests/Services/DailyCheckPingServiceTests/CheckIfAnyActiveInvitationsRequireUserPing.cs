using System.Collections.Generic;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DevBetterWeb.Tests.Services.DailyCheckPingServiceTests;

public class CheckIfAnyActiveInvitationsRequireUserPing
{
  private readonly Mock<IRepository<Invitation>> _repository = new();
  private readonly Mock<IRepository<Member>> _membrRepository = new();
  private readonly Mock<IEmailService> _emailService = new();
  private readonly Mock<UserManager<ApplicationUser>> _userManager;

  private DailyCheckPingService _service;

  private List<Invitation> _testlist = new();
  private InvitationBuilder _builder = new();

  public CheckIfAnyActiveInvitationsRequireUserPing()
  {
    var store = new Mock<IUserStore<ApplicationUser>>();
    _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

    _service = new DailyCheckPingService(_repository.Object, _membrRepository.Object, _emailService.Object, _userManager.Object);
  }

  [Fact]
  public void ReturnsEmptyListGivenEmptyList()
  {
    var list = _service.CheckIfAnyActiveInvitationsRequireUserPing(_testlist);

    Assert.Empty(list);
  }

  //[Fact]
  // TODO: Diagnose and fix
  //public void ReturnsListIfListContainsActiveInvitationCreated2DaysAgoWithNoUserPingDate()
  //{
  //  _testlist.Add(_builder.WithDateCreatedGivenDaysAgo(2)
  //    .Build());

  //  var list = _service.CheckIfAnyActiveInvitationsRequireUserPing(_testlist);

  //  Assert.Equal(_testlist, list);
  //}

  [Fact]
  public void ReturnsEmptyListIfListContainsActiveInvitationCreated2DaysAgoWithUserPingDateOfToday()
  {
    _testlist.Add(_builder.WithDateCreatedGivenDaysAgo(2)
      .WithDateOfUserPingGivenDaysAgo(0)
      .Build());

    var list = _service.CheckIfAnyActiveInvitationsRequireUserPing(_testlist);

    Assert.Empty(list);
  }

  [Fact]
  public void ReturnsEmptyListIfListContainsActiveInvitationCreated5DaysAgoWithUserPingDateOf3DaysAgo()
  {
    _testlist.Add(_builder.WithDateCreatedGivenDaysAgo(5)
     .WithDateOfUserPingGivenDaysAgo(3)
     .Build());

    var list = _service.CheckIfAnyActiveInvitationsRequireUserPing(_testlist);

    Assert.Empty(list);
  }

  //[Theory]
  // TODO: Diagnose and fix
  //[MemberData(nameof(GetInvitations))]
  //public void ReturnsListWithOneInviteGivenListWithOneValidAndOneInvalid(Invitation valid, Invitation invalid)
  //{
  //  _testlist.Add(valid);
  //  _testlist.Add(invalid);

  //  var list = _service.CheckIfAnyActiveInvitationsRequireUserPing(_testlist);

  //  Assert.Contains(valid, list);
  //  Assert.DoesNotContain(invalid, list);
  //}

  public static IEnumerable<object[]> GetInvitations()
  {
    yield return new object[]
    {
        new InvitationBuilder().WithDateCreatedGivenDaysAgo(3)
        .Build(),
        new InvitationBuilder().WithDateCreatedGivenDaysAgo(5)
        .WithDateOfUserPingGivenDaysAgo(3)
        .Build()
    };
    yield return new object[]
    {
        new InvitationBuilder().WithDateCreatedGivenDaysAgo(3)
        .Build(),
        new InvitationBuilder().AndDeactivated()
        .Build()
    };
  }
}
