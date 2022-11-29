using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class MyProfileLinksModel : PageModel
{
#nullable disable
  [BindProperty]
  public UserLinksUpdateModel UserLinksUpdateModel { get; set; }

#nullable enable

  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IMemberRegistrationService _memberRegistrationService;
  private readonly IRepository<Member> _memberRepository;

  public MyProfileLinksModel(UserManager<ApplicationUser> userManager,
      IMemberRegistrationService memberRegistrationService,
      IRepository<Member> memberRepository)
  {
    _userManager = userManager;
    _memberRegistrationService = memberRegistrationService;
    _memberRepository = memberRepository;
  }

  public async Task OnGetAsync()
  {
    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName!);

    var spec = new MemberByUserIdWithBooksReadSpec(applicationUser!.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(spec);

    if (member == null)
    {
      member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
    }

    UserLinksUpdateModel = new UserLinksUpdateModel(member);
  }

  public async Task OnPost()
  {
    if (!ModelState.IsValid) return;
    // TODO: consider only getting the user alias not the whole URL for social media links
    // TODO: assess risk of XSS attacks and how to mitigate

    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName!);

    var spec = new MemberByUserIdWithBooksReadSpec(applicationUser!.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(spec);
    if (member is null) throw new MemberNotFoundException(applicationUser.Id);

    member.UpdateLinks(UserLinksUpdateModel.BlogUrl, UserLinksUpdateModel.CodinGameUrl, UserLinksUpdateModel.GithubUrl, UserLinksUpdateModel.LinkedInUrl,
              UserLinksUpdateModel.OtherUrl, UserLinksUpdateModel.TwitchUrl, UserLinksUpdateModel.YouTubeUrl, UserLinksUpdateModel.TwitterUrl);

    await _memberRepository.UpdateAsync(member);
  }
}
