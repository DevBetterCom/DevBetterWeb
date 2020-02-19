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

namespace DevBetterWeb.Web.Pages.User
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        public UserDetailsViewModel UserDetailsViewModel { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _appDbContext;

        public DetailsModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._appDbContext = appDbContext;
        }

        public async Task OnGet(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                BadRequest();
            }

            var member = _appDbContext.Members.FirstOrDefault(x => x.UserId == user.Id);

            if (member == null)
            {
                BadRequest();

            }

            UserDetailsViewModel = new UserDetailsViewModel(member);
        }
    }
}