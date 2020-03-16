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
            var member = await _repository.GetBySpecAsync(spec);

            if (member == null)
            {
                member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
            }

            UserProfileUpdateModel = new UserProfileUpdateModel(member);
        }

        public async Task OnPost()
        {
            if (!ModelState.IsValid) return;

            var currentUserName = User.Identity.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

            var spec = new MemberByUserIdSpec(applicationUser.Id);
            var member = await _repository.GetBySpecAsync(spec);

            member.UpdateName(UserProfileUpdateModel.FirstName, UserProfileUpdateModel.LastName);
            member.UpdateAboutInfo(UserProfileUpdateModel.AboutInfo);
            member.UpdateAddress(UserProfileUpdateModel.Address);
            member.UpdateLinks(UserProfileUpdateModel.BlogUrl, UserProfileUpdateModel.GithubUrl, UserProfileUpdateModel.LinkedInUrl,
                UserProfileUpdateModel.OtherUrl, UserProfileUpdateModel.TwitchUrl, UserProfileUpdateModel.TwitterUrl);

            await _repository.UpdateAsync(member);
        }
    }
}