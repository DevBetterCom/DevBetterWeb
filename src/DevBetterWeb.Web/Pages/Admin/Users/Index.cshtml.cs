using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.Admin.Users;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class IndexModel : PageModel
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;

  public IndexModel(UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager)
  {
    _userManager = userManager;
    _roleManager = roleManager;
  }

  public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
  public List<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
  public List<UserWithRoles> UsersWithRole { get; set; } = new List<UserWithRoles>();
  public IdentityRole? Role { get; private set; }

  public async Task<IActionResult> OnGetAsync()
  {
    Users = await _userManager.Users.ToListAsync();
    Roles = await _roleManager.Roles.ToListAsync();

    foreach (var user in Users)
    {
      var roles = await _userManager.GetRolesAsync(user);

      if (Role != null && roles.Contains(Role!.Name!))
      {
        var userWithRoles = new UserWithRoles(user, roles.ToList());
        UsersWithRole.Add(userWithRoles);
      }
      else if (Role == null)
      {
        var userWithRoles = new UserWithRoles(user, roles.ToList());
        UsersWithRole.Add(userWithRoles);
      }

    }

    UsersWithRole = UsersWithRole.OrderBy(x => x.User.DateCreated).ToList();

		return Page();
  }

  public void SetPageRole(IdentityRole role)
  {
    Role = role;
  }

  public class UserWithRoles
  {
    public ApplicationUser User { get; set; }
    public List<string> Roles { get; set; }

    public UserWithRoles(ApplicationUser user, List<string> roles)
    {
      User = user;
      Roles = roles;
    }
  }
}
