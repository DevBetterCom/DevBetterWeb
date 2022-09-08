using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User.MyProfile;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class IndexModel : PageModel
{
#nullable disable
  [BindProperty]
  public UserProfileViewModel UserProfileViewModel { get; set; }
  public string AvatarUrl { get; set; }

  public List<Book> Books { get; set; } = new List<Book>();

  public string AlumniProgressPercentage { get; set; }
  public MemberSubscriptionPercentBarViewModel Model { get; set; }

#nullable enable

  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IMemberRegistrationService _memberRegistrationService;
  private readonly IRepository<Member> _memberRepository;
  private readonly IRepository<Book> _bookRepository;
  private readonly IMemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;

  public IndexModel(UserManager<ApplicationUser> userManager,
      IMemberRegistrationService memberRegistrationService,
      IRepository<Member> memberRepository,
      IRepository<Book> bookRepository,
      IMemberSubscriptionPeriodCalculationsService memberSubscriptionPeriodCalculationsService)
  {
    _userManager = userManager;
    _memberRegistrationService = memberRegistrationService;
    _memberRepository = memberRepository;
    _bookRepository = bookRepository;
    _memberSubscriptionPeriodCalculationsService = memberSubscriptionPeriodCalculationsService;
  }

  public async Task OnGetAsync()
  {
    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName);
    AvatarUrl = string.Format(Constants.AVATAR_IMGURL_FORMAT_STRING, applicationUser.Id);

    var spec = new MemberByUserIdWithBooksReadAndMemberSubscriptionsSpec(applicationUser.Id);
		Member? member = await _memberRepository.FirstOrDefaultAsync(spec);

    if (member == null)
    {
      member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
    }

    Books = await _bookRepository.ListAsync();

    int percentage = _memberSubscriptionPeriodCalculationsService.GetPercentageProgressToAlumniStatus(member);

    Model = new MemberSubscriptionPercentBarViewModel(percentage);

    UserProfileViewModel = new UserProfileViewModel(member);
  }

  public bool GetIsAlumni()
  {
    return User.IsInRole(AuthConstants.Roles.ALUMNI);
  }
}
