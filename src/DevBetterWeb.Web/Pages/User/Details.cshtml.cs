using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User
{
    public class DetailsModel : PageModel
    {
        public UserDetailsViewModel UserDetailsViewModel { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DetailsModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public async Task OnGet(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                BadRequest();
            }

            UserDetailsViewModel = new UserDetailsViewModel(user);
        }
    }
}