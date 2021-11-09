using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Services;

public class DefaultUserRoleManagerService : IUserRoleManager
{
  private readonly UserManager<ApplicationUser> _userManager;

  public DefaultUserRoleManagerService(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public async Task AddUserToRoleAsync(string userId, string roleName)
  {
    var user = await _userManager.FindByIdAsync(userId);
    await _userManager.AddToRoleAsync(user, Constants.ALUMNI_ROLE_NAME);
  }
}
