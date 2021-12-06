using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Services;

public class UserLookupService : IUserLookupService
{
  private readonly UserManager<ApplicationUser> _userManager;

  public UserLookupService(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public async Task<string> FindUserIdByEmailAsync(string email)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null) throw new UserWithEmailAddressNotFoundException(email);

    return user.Id;
  }

  public async Task<bool> FindUserIsMemberByEmailAsync(string email)
  {
    var user = await _userManager.FindByEmailAsync(email);

    if (user == null)
    {
      return false;
    }

    var roles = await _userManager.GetRolesAsync(user);

    return roles.Any(role => role.Equals(Constants.MEMBER_ROLE_NAME));
  }

  public async Task<bool> FindUserIsAlumniByUserIdAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);

    if (user == null)
    {
      return false;
    }

    var roles = await _userManager.GetRolesAsync(user);

    return roles.Any(role => role.Equals(Constants.ALUMNI_ROLE_NAME));
  }
}
