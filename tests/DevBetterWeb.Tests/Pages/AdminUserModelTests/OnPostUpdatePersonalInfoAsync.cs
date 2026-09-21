using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Pages.Admin;
using DevBetterWeb.Web.Pages.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace DevBetterWeb.Tests.Pages.AdminUserModelTests;

public class OnPostUpdatePersonalInfoAsync
{
  private const string UserId = "user-123";

  private readonly IRepository<Member> _memberRepository = Substitute.For<IRepository<Member>>();
  private readonly UserManager<ApplicationUser> _userManager = UserManagerHelpers.CreateSubstitute();
  private readonly Member _member = MemberHelpers.CreateWithInternalConstructor();
  private readonly UserModel _pageModel;

  public OnPostUpdatePersonalInfoAsync()
  {
    _memberRepository.FirstOrDefaultAsync(Arg.Any<MemberByUserIdSpec>(), Arg.Any<CancellationToken>())
      .Returns(_member);
    _userManager.FindByIdAsync(UserId).Returns(new ApplicationUser { Id = UserId });

    var roleManager = Substitute.For<RoleManager<IdentityRole>>(
      Substitute.For<IRoleStore<IdentityRole>>(), null!, null!, null!, null!);

    _pageModel = new UserModel(NullLogger<UserModel>.Instance,
      _userManager,
      roleManager,
      Substitute.For<IUserRoleMembershipService>(),
      Substitute.For<IMemberRegistrationService>(),
      _memberRepository,
      Substitute.For<IRepository<MemberSubscription>>(),
      Substitute.For<IRepository<MemberSubscriptionPlan>>(),
      Substitute.For<IUserEmailConfirmationService>(),
      Substitute.For<IInvoiceHandlerListService>(),
      Substitute.For<IMapper>());
    _pageModel.PageContext = new PageContext { HttpContext = new DefaultHttpContext() };
  }

  private void GivenBindingErrorsForEmptyAddressFields()
  {
    // Mirrors what model binding reports for the [Required] address fields when they are left blank.
    foreach (var field in new[] { "Address", "City", "Country", "PostalCode" })
    {
      _pageModel.ModelState.AddModelError($"UserPersonalUpdateModel.{field}", $"The {field} field is required.");
    }
  }

  [Fact]
  public async Task SavesNameAndEmailGivenNoShippingAddress()
  {
    _pageModel.UserPersonalUpdateModel = new UserPersonalUpdateModel
    {
      FirstName = "Kajan",
      LastName = "Smith",
      Email = "kajan@example.com"
    };
    GivenBindingErrorsForEmptyAddressFields();

    var result = await _pageModel.OnPostUpdatePersonalInfoAsync(UserId);

    Assert.IsType<RedirectToPageResult>(result);
    Assert.Equal("Kajan", _member.FirstName);
    Assert.Equal("kajan@example.com", _member.Email);
    Assert.Null(_member.ShippingAddress);
    await _memberRepository.Received(1).UpdateAsync(_member, Arg.Any<CancellationToken>());
    await _userManager.Received(1).UpdateAsync(Arg.Is<ApplicationUser>(u => u.Email == "kajan@example.com"));
  }

  [Fact]
  public async Task RedisplaysFormWithSubmittedValuesGivenPartialShippingAddress()
  {
    _pageModel.UserPersonalUpdateModel = new UserPersonalUpdateModel
    {
      FirstName = "Kajan",
      LastName = "Smith",
      City = "Vavuniya"
    };
    GivenBindingErrorsForEmptyAddressFields();

    var result = await _pageModel.OnPostUpdatePersonalInfoAsync(UserId);

    Assert.IsType<PageResult>(result);
    Assert.False(_pageModel.ModelState.IsValid);
    Assert.Equal("Vavuniya", _pageModel.UserPersonalUpdateModel.City);
    Assert.Equal(UserId, _pageModel.UserId);
    await _memberRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
  }

  [Fact]
  public async Task RedisplaysFormInsteadOfBadRequestGivenMissingLastName()
  {
    _pageModel.UserPersonalUpdateModel = new UserPersonalUpdateModel { FirstName = "Kajan" };
    _pageModel.ModelState.AddModelError("UserPersonalUpdateModel.LastName", "The LastName field is required.");

    var result = await _pageModel.OnPostUpdatePersonalInfoAsync(UserId);

    Assert.IsType<PageResult>(result);
    await _memberRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
  }

  [Fact]
  public async Task RedisplaysFormWithErrorGivenNoMemberRecord()
  {
    _memberRepository.FirstOrDefaultAsync(Arg.Any<MemberByUserIdSpec>(), Arg.Any<CancellationToken>())
      .Returns(null);
    _pageModel.UserPersonalUpdateModel = new UserPersonalUpdateModel { FirstName = "Kajan", LastName = "Smith" };

    var result = await _pageModel.OnPostUpdatePersonalInfoAsync(UserId);

    Assert.IsType<PageResult>(result);
    Assert.False(_pageModel.ModelState.IsValid);
  }
}
