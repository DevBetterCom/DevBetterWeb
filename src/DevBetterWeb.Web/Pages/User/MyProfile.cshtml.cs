using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.User
{
    [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
    public class MyProfileModel : PageModel
    {
#nullable disable
        [BindProperty]
        public UserProfileUpdateModel UserProfileUpdateModel { get; set; }
#nullable enable

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemberRegistrationService _memberRegistrationService;
        private readonly IRepository _repository;

        public MyProfileModel(UserManager<ApplicationUser> userManager, 
            IMemberRegistrationService memberRegistrationService,
            IRepository repository)
        {
            _userManager = userManager;
            _memberRegistrationService = memberRegistrationService;
            _repository = repository;
        }

        public async Task OnGetAsync()
        {
            var currentUserName = User.Identity.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

            var spec = new MemberByUserIdSpec(applicationUser.Id);
            var member = await _repository.GetAsync(spec);

            if (member == null)
            {
                member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
            }

            UserProfileUpdateModel = new UserProfileUpdateModel(member);
        }

        public async Task OnPost()
        {
            if (!ModelState.IsValid) return;
            // TODO: consider only getting the user alias not the whole URL for social media links
            // TODO: assess risk of XSS attacks and how to mitigate

            var currentUserName = User.Identity.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

            var spec = new MemberByUserIdSpec(applicationUser.Id);
            var member = await _repository.GetAsync(spec);

            member.UpdateName(UserProfileUpdateModel.FirstName, UserProfileUpdateModel.LastName); 
            member.UpdatePEInfo(UserProfileUpdateModel.PEFriendCode, UserProfileUpdateModel.PEUsername);
            member.UpdateAboutInfo(UserProfileUpdateModel.AboutInfo);
            member.UpdateAddress(UserProfileUpdateModel.Address);
            member.UpdateLinks(UserProfileUpdateModel.BlogUrl, UserProfileUpdateModel.GithubUrl, UserProfileUpdateModel.LinkedInUrl,
                UserProfileUpdateModel.OtherUrl, UserProfileUpdateModel.TwitchUrl, UserProfileUpdateModel.YouTubeUrl, UserProfileUpdateModel.TwitterUrl);

            await _repository.UpdateAsync(member);
        }
    }
}