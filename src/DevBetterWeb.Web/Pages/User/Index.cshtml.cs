using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        
        public List<KeyValuePair<string, string>> UserIdsAndNames { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
        public async Task OnGet()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);

            UserIdsAndNames = usersInRole
                .OrderBy(x => x.LastName)
                .Select(x => new KeyValuePair<string, string>(x.Id, x.UserFullName()))                
                .ToList();

        }
    }

   
}