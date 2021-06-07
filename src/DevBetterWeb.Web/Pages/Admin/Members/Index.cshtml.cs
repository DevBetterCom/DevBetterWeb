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

namespace DevBetterWeb.Web.Pages.Admin.Members
{
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
    public List<UserWithRoles> UsersWithMemberRole { get; set; } = new List<UserWithRoles>();
    public IdentityRole? Member { get; private set; } = new IdentityRole(AuthConstants.Roles.MEMBERS);

    public async Task<IActionResult> OnGetAsync()
    {
      Users = await _userManager.Users.ToListAsync();

      foreach (var user in Users)
      {
        var roles = await _userManager.GetRolesAsync(user);

        if (Member != null && roles.Contains(Member.Name))
        {
          var userWithRoles = new UserWithRoles(user, roles.ToList());
          UsersWithMemberRole.Add(userWithRoles);
        }
      }


      return Page();
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
}
