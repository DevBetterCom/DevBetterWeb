using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.User
{

    [Authorize]
    public class MyProfileModel : PageModel
    {
        [BindProperty]
        public UserProfileUpdateModel UserProfileUpdateModel { get; set; }


            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly AppDbContext _appDbContext;

        public MyProfileModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
            {
                this._userManager = userManager;
                this._roleManager = roleManager;
                this._appDbContext = appDbContext;
        }

       

        public async Task OnGet()
        {
            var currentUserName = User.Identity.Name;
            var applicationUser = await _userManager.FindByNameAsync(currentUserName);

            var member = await _appDbContext.Members.FirstOrDefaultAsync(x => x.UserId == applicationUser.Id);

            if (member == null)
            {
                member = new Core.Entities.Member() 
                { 
                    UserId = applicationUser.Id
                };
                _appDbContext.Members.Add(member);
                _appDbContext.SaveChanges();
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

            var member = _appDbContext.Members.First(x => x.UserId == applicationUser.Id);

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

            _appDbContext.SaveChanges();
        }
    }
}