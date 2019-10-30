using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevBetterWeb.Web.Pages.Admin
{
    [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
    public class RoleModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;

        }
        public IdentityRole Role { get; set; }
        public List<ApplicationUser> UsersInRole { get; set; }
        public List<SelectListItem> UsersNotInRole { get; set; }

        public async Task<IActionResult> OnGetAsync(string roleId)
        {
            var role = _roleManager.Roles.Single(x => x.Id == roleId);

            if (role == null)
            {
                return BadRequest();
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            var userIdsInRole = usersInRole.Select(X => X.Id).ToList();
            var usersNotInRole = _userManager.Users.Where(x => !userIdsInRole.Contains(x.Id)).ToList();

            Role = role;
            UsersInRole = usersInRole.ToList();
            UsersNotInRole = usersNotInRole.Select(x => new SelectListItem(x.Email, x.Id)).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAddUserToRoleAsync(string userId, string roleId)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == roleId);

            if (user == null || role == null)
            {
                return BadRequest();
            }

            await _userManager.AddToRoleAsync(user, role.Name);
            return RedirectToPage("./Role", new { roleId = roleId });
        }

        public async Task<IActionResult> OnPostRemoveUserFromRole(string userId, string roleId)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == roleId);

            if (user == null || role == null)
            {
                return BadRequest();
            }

            await _userManager.RemoveFromRoleAsync(user, role.Name);
            return RedirectToPage("./Role", new { roleId = roleId });
        }
    }
}