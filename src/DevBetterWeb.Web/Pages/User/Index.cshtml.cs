using System;
using System.Collections.Generic;
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
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _appDbContext;

        public List<KeyValuePair<string, string>> UserIdsAndNames { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._appDbContext = appDbContext;
        }
        public async Task OnGet()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);

            var userIds = usersInRole.Select(x => x.Id).ToList();

            var members = await _appDbContext.Members.AsNoTracking().Where(x => userIds.Contains(x.UserId)).ToListAsync();

            UserIdsAndNames = members
                .OrderBy(x => x.LastName)
                .Select(x => new KeyValuePair<string, string>(x.UserId, x.UserFullName()))                
                .ToList();

        }
    }

   
}