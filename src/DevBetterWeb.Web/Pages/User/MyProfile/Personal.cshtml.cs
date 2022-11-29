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
public class MyProfilePersonalModel : PageModel
{
#nullable disable
  [BindProperty]
  public UserPersonalUpdateModel UserPersonalUpdateModel { get; set; }

#nullable enable

  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IMemberRegistrationService _memberRegistrationService;
  private readonly IRepository<Member> _memberRepository;
  private readonly AppDbContext _appDbContext;

  public MyProfilePersonalModel(UserManager<ApplicationUser> userManager,
      IMemberRegistrationService memberRegistrationService,
      IRepository<Member> memberRepository,
      AppDbContext appDbContext)
  {
    _userManager = userManager;
    _memberRegistrationService = memberRegistrationService;
    _memberRepository = memberRepository;
    _appDbContext = appDbContext;
  }

  public async Task OnGetAsync()
  {
    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName!);

    var spec = new MemberByUserIdSpec(applicationUser!.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(spec);

    //var books = await _appDbContext.Books
    //    .Include(book => book.MembersWhoHaveRead)
    //    .ToListAsync();

    //var members = await _appDbContext.Members
    //    .Include(member => member.BooksRead)
    //    .ToListAsync();



    if (member == null)
    {
      member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
    }

    UserPersonalUpdateModel = new UserPersonalUpdateModel(member);
  }

  public async Task OnPost()
  {
    if (!ModelState.IsValid) return;
    // TODO: consider only getting the user alias not the whole URL for social media links
    // TODO: assess risk of XSS attacks and how to mitigate

    var currentUserName = User.Identity!.Name;
    var applicationUser = await _userManager.FindByNameAsync(currentUserName!);

    var spec = new MemberByUserIdSpec(applicationUser!.Id);
    var member = await _memberRepository.FirstOrDefaultAsync(spec);
    if (member is null) throw new MemberNotFoundException(applicationUser.Id);

    member.UpdateName(UserPersonalUpdateModel.FirstName, UserPersonalUpdateModel.LastName);
    member.UpdatePEInfo(UserPersonalUpdateModel.PEFriendCode, UserPersonalUpdateModel.PEUsername);
    member.UpdateAboutInfo(UserPersonalUpdateModel.AboutInfo);
    member.UpdateAddress(UserPersonalUpdateModel.Address);
    member.UpdateBirthday(UserPersonalUpdateModel.BirthdayDay, UserPersonalUpdateModel.BirthdayMonth);
    member.UpdateDiscord(UserPersonalUpdateModel.DiscordUsername);
    member.UpdateEmail(UserPersonalUpdateModel.Email);

    await _memberRepository.UpdateAsync(member);

    if (!string.IsNullOrEmpty(UserPersonalUpdateModel.Email))
    {
	    applicationUser.Email = UserPersonalUpdateModel.Email;
	    await _userManager.UpdateAsync(applicationUser);
    }
	}
}
