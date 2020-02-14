﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.Admin
{
    [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public List<ApplicationUser> Users { get; set; }
        public List<IdentityRole> Roles { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
           Users = await _userManager.Users.ToListAsync();
           Roles = await _roleManager.Roles.ToListAsync();

            return Page();
        }
    }
}