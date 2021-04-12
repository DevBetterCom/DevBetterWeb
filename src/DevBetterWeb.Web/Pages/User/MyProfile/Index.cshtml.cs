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

namespace DevBetterWeb.Web.Pages.User.MyProfile
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
  public class IndexModel : PageModel
  {
#nullable disable
    [BindProperty]
    public UserProfileViewModel UserProfileViewModel { get; set; }
    public string AvatarUrl { get; set; }

    public List<Book> Books { get; set; } = new List<Book>();

#nullable enable

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMemberRegistrationService _memberRegistrationService;
    private readonly IRepository _repository;

    public IndexModel(UserManager<ApplicationUser> userManager,
        IMemberRegistrationService memberRegistrationService,
        IRepository repository)
    {
      _userManager = userManager;
      _memberRegistrationService = memberRegistrationService;
      _repository = repository;
    }

    public async Task OnGetAsync()
    {
      var currentUserName = User.Identity!.Name;
      var applicationUser = await _userManager.FindByNameAsync(currentUserName);
      AvatarUrl = string.Format(Constants.AVATAR_IMGURL_FORMAT_STRING, applicationUser.Id);

      var spec = new MemberByUserIdWithBooksReadSpec(applicationUser.Id);
      var member = await _repository.GetAsync(spec);

      if (member == null)
      {
        member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
      }

      Books = await _repository.ListAsync<Book>();

      UserProfileViewModel = new UserProfileViewModel(member);
    }

    //public async Task OnPost()
    //{
    //    if (!ModelState.IsValid) return;
    //    // TODO: consider only getting the user alias not the whole URL for social media links
    //    // TODO: assess risk of XSS attacks and how to mitigate

    //    var currentUserName = User.Identity!.Name;
    //    var applicationUser = await _userManager.FindByNameAsync(currentUserName);

    //    var spec = new MemberByUserIdWithBooksReadSpec(applicationUser.Id);
    //    var member = await _repository.GetAsync(spec);

    //    if (UserProfileUpdateModel.AddedBook.HasValue)
    //    {
    //        AddedBook = await _repository.GetByIdAsync<Book>(UserProfileUpdateModel.AddedBook.Value);

    //        member.AddBookRead(AddedBook);
    //    }

    //    if (UserProfileUpdateModel.RemovedBook.HasValue)
    //    {
    //        RemovedBook = await _repository.GetByIdAsync<Book>(UserProfileUpdateModel.RemovedBook.Value);

    //        member.RemoveBookRead(RemovedBook);
    //    }

    //    member.UpdateName(UserProfileUpdateModel.FirstName, UserProfileUpdateModel.LastName); 
    //    member.UpdatePEInfo(UserProfileUpdateModel.PEFriendCode, UserProfileUpdateModel.PEUsername);
    //    member.UpdateAboutInfo(UserProfileUpdateModel.AboutInfo);
    //    member.UpdateAddress(UserProfileUpdateModel.Address);
    //    member.UpdateLinks(UserProfileUpdateModel.BlogUrl, UserProfileUpdateModel.GithubUrl, UserProfileUpdateModel.LinkedInUrl,
    //        UserProfileUpdateModel.OtherUrl, UserProfileUpdateModel.TwitchUrl, UserProfileUpdateModel.YouTubeUrl, UserProfileUpdateModel.TwitterUrl);

    //    // this is trying to add to bookmember database - why?
    //    await _repository.UpdateAsync(member);
    //}
  }
}
