using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
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
        private readonly AppDbContext _appDbContext;

        public MyProfileModel(UserManager<ApplicationUser> userManager, 
            AppDbContext appDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        public async Task OnGetAsync()
        {
            var currentUserName = User.Identity.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

            var member = await _appDbContext.Members
                .FirstOrDefaultAsync(member => member.UserId == applicationUser.Id);

            if (member == null)
            {
                member = new Core.Entities.Member()
                {
                    UserId = applicationUser.Id
                };
                _appDbContext.Members!.Add(member);
                await _appDbContext.SaveChangesAsync();
            }

            UserProfileUpdateModel = new UserProfileUpdateModel(member);
        }

        public async Task OnPost()
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            var currentUserName = User.Identity.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

            var member = _appDbContext.Members
                .First(member => member.UserId == applicationUser.Id);

            // TODO: Replace with AutoMapper or Extension method
            member.FirstName = UserProfileUpdateModel.FirstName;
            member.LastName = UserProfileUpdateModel.LastName;
            member.AboutInfo = UserProfileUpdateModel.AboutInfo;
            member.Address = UserProfileUpdateModel.Address;
            member.BlogUrl = UserProfileUpdateModel.BlogUrl;
            member.GithubUrl = UserProfileUpdateModel.GithubUrl;
            member.LinkedInUrl = UserProfileUpdateModel.LinkedInUrl;
            member.TwitterUrl = UserProfileUpdateModel.TwitterUrl;
            member.TwitchUrl = UserProfileUpdateModel.TwitchUrl;
            member.OtherUrl = UserProfileUpdateModel.OtherUrl;

            await _appDbContext.SaveChangesAsync();

            // TODO: Raise event that someone updated profile, so email notification to admins can go out
        }
    }


}