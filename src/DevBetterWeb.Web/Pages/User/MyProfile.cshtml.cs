using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User
{

    [Authorize]
    public class MyProfileModel : PageModel
    {
        [BindProperty]
        public UserProfileUpdateModel UserProfileUpdateModel { get; set; }


            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public MyProfileModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
            {
                this._userManager = userManager;
                this._roleManager = roleManager;
            }

       

        public async Task OnGet()
        {
            var currentUserName = User.Identity.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

            UserProfileUpdateModel = new UserProfileUpdateModel(applicationUser);
        }

        public async Task OnPost()
        {

            if (!ModelState.IsValid)
            {
                return;
            }
            
            var currentUserName = User.Identity.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

           

            applicationUser.FirstName = UserProfileUpdateModel.FirstName;
            applicationUser.LastName = UserProfileUpdateModel.LastName;
            applicationUser.AboutInfo = UserProfileUpdateModel.AboutInfo;
            applicationUser.Address = UserProfileUpdateModel.Address;
            applicationUser.BlogUrl = UserProfileUpdateModel.BlogUrl;
            applicationUser.GithubUrl = UserProfileUpdateModel.GithubUrl;
            applicationUser.LinkedInUrl = UserProfileUpdateModel.LinkedInUrl;
            applicationUser.TwitterUrl = UserProfileUpdateModel.TwitterUrl;
            applicationUser.TwitchUrl = UserProfileUpdateModel.TwitchUrl;
            applicationUser.OtherUrl = UserProfileUpdateModel.OtherUrl;

            await _userManager.UpdateAsync(applicationUser);
        }
    }
}