using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.ValueObjects;
using DevBetterWeb.Infrastructure.Services;
using Moq;
using Xunit;

namespace DevBetterWeb.Tests.Services.AlumniGraduationServiceTests;

public class CheckIfAnyMemberGraduating
{
  private readonly Mock<IUserLookupService> userLookupService;
  private readonly Mock<IRepository<Member>> repository;
  private readonly Mock<IGraduationCommunicationsService> graduationCommunications;
  private readonly Mock<IUserRoleManager> userManager;

  private const int DAYS_IN_TWO_YEARS = 365 * 2;

  private AlumniGraduationService service;

  public CheckIfAnyMemberGraduating()
  {
    userLookupService = new Mock<IUserLookupService>();
    repository = new Mock<IRepository<Member>>();
    graduationCommunications = new Mock<IGraduationCommunicationsService>();
    userManager = new Mock<IUserRoleManager>();
    service = new AlumniGraduationService(userLookupService.Object, repository.Object, graduationCommunications.Object, userManager.Object);
  }

  [Fact]
  public async Task ReturnsEmptyListGivenEmptyList()
  {
    var testlist = new List<Member>();

    var list = await service.CheckIfAnyMemberGraduating(testlist);

    Assert.Empty(list);
  }

  [Fact]
  public async Task ReturnsSameListGivenListWithAllMembersGraduating()
  {
    var testlist = new List<Member>();
    var graduatingMember = GetGraduatingMember();
    testlist.Add(graduatingMember);

    userLookupService.Setup(u => u.FindUserIsAlumniByUserIdAsync(graduatingMember.UserId)).ReturnsAsync(false);

    var list = await service.CheckIfAnyMemberGraduating(testlist);

    Assert.Contains(graduatingMember, list);
  }

  [Fact]
  public async Task ReturnsOnlyGraduatingMembersGivenListWithSomeMembersGraduating()
  {
    var testlist = new List<Member>();

    var graduatingMember = GetGraduatingMember();
    testlist.Add(graduatingMember);

    var nonGraduatingMember = GetNonGraduatingMember();
    testlist.Add(nonGraduatingMember);

    userLookupService.Setup(u => u.FindUserIsAlumniByUserIdAsync(graduatingMember.UserId)).ReturnsAsync(false);

    var list = await service.CheckIfAnyMemberGraduating(testlist);

    Assert.Contains(graduatingMember, list);
    Assert.DoesNotContain(nonGraduatingMember, list);
  }

  [Fact]
  public async Task ReturnsEmptyListGivenListWithNoMembersGraduating()
  {
    var testlist = new List<Member>();
    var nonGraduatingMember = GetNonGraduatingMember();
    testlist.Add(nonGraduatingMember);

    var list = await service.CheckIfAnyMemberGraduating(testlist);

    Assert.Empty(list);
  }

  [Fact]
  public async Task ReturnsEmptyListGivenListOfAlumni()
  {
    var testlist = new List<Member>();
    var alum = GetGraduatingMember();
    testlist.Add(alum);

    userLookupService.Setup(u => u.FindUserIsAlumniByUserIdAsync(alum.UserId)).ReturnsAsync(true);

    var list = await service.CheckIfAnyMemberGraduating(testlist);

    Assert.Empty(list);
  }

  [Fact]
  public async Task ReturnsEmptyListGivenListWithMemberOneDayFromGraduation()
  {
    var testlist = new List<Member>();
    var member = new Member();
    var dates = new DateTimeRange(DateTime.Today.AddDays(DAYS_IN_TWO_YEARS * -1).AddDays(1), DateTime.Today.AddDays(1));
    var nearGraduationSubscription = new MemberSubscription(member.Id, SubscriptionHelpers.TEST_MEMBER_PLAN_ID, dates);
    member.MemberSubscriptions.Add(nearGraduationSubscription);
    testlist.Add(member);

    userLookupService.Setup(u => u.FindUserIsAlumniByUserIdAsync(member.UserId)).ReturnsAsync(false);

    var list = await service.CheckIfAnyMemberGraduating(testlist);

    Assert.Empty(list);
  }

  private Member GetGraduatingMember()
  {
    var graduatingMember = new Member();
    var dates = new DateTimeRange(DateTime.Now.AddYears(-2).AddDays(-1), DateTime.Now);
    var graduationSubscription = new MemberSubscription(graduatingMember.Id, SubscriptionHelpers.TEST_MEMBER_PLAN_ID, dates);
    graduatingMember.MemberSubscriptions.Add(graduationSubscription);

    return graduatingMember;
  }

  private Member GetNonGraduatingMember()
  {
    var nonGraduatingMember = new Member();
    var dates = new DateTimeRange(DateTime.Now.AddYears(-1), DateTime.Now.AddYears(1));
    var nonGraduationSubscription = new MemberSubscription(nonGraduatingMember.Id, SubscriptionHelpers.TEST_MEMBER_PLAN_ID, dates);
    nonGraduatingMember.MemberSubscriptions.Add(nonGraduationSubscription);

    return nonGraduatingMember;
  }
}
