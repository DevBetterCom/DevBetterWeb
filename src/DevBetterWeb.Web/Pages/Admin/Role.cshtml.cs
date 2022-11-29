using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevBetterWeb.Web.Pages.Admin;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class RoleModel : PageModel
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly IUserRoleMembershipService _userRoleMembershipService;

  public RoleModel(UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager,
      IUserRoleMembershipService userRoleMembershipService)
  {
    _userManager = userManager;
    _roleManager = roleManager;
    _userRoleMembershipService = userRoleMembershipService;
  }
  public IdentityRole? Role { get; set; }
  public List<ApplicationUser> UsersInRole { get; set; } = new List<ApplicationUser>();
  public List<SelectListItem> UsersNotInRole { get; set; } = new List<SelectListItem>();

  public async Task<IActionResult> OnGetAsync(string roleId)
  {
    var role = _roleManager.Roles.Single(x => x.Id == roleId);

    if (role == null)
    {
      return BadRequest();
    }

    var usersInRole = await _userManager.GetUsersInRoleAsync(role!.Name!);

    var userIdsInRole = usersInRole.Select(X => X.Id).ToList();
    var usersNotInRole = _userManager.Users.Where(x => !userIdsInRole.Contains(x.Id)).ToList();

    Role = role;
    UsersInRole = usersInRole.ToList();
    UsersNotInRole = usersNotInRole.Select(x => new SelectListItem(x.Email, x.Id)).ToList();

    return Page();
  }

  public async Task<IActionResult> OnPostAddUserToRoleAsync(string userId, string roleId)
  {
    await _userRoleMembershipService.AddUserToRoleAsync(userId, roleId);

    return RedirectToPage("./Role", new { roleId });
  }

  public async Task<IActionResult> OnPostRemoveUserFromRole(string userId, string roleId)
  {
    await _userRoleMembershipService.RemoveUserFromRoleAsync(userId, roleId);

    return RedirectToPage("./Role", new { roleId });
  }
}
