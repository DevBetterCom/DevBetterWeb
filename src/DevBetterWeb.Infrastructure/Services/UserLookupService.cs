using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using DevBetterWeb.Core;

namespace DevBetterWeb.Infrastructure.Services
{
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

      var id = user.Id;

      return id;
    }

    public async Task<bool> FindUserIsMemberByEmailAsync(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);

      if(user == null)
      {
        return false;
      }

      var roles = await _userManager.GetRolesAsync(user);

      return roles.Any(role => role.Equals(Constants.MEMBER_ROLE_NAME));
    }
  }
}
