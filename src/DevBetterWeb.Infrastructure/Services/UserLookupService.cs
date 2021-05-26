using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;

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

      var memberRoleName = Core.Constants.MEMBER_ROLE_NAME;

      bool userIsMember = false;

      foreach (var role in roles)
      {
        if (role.Equals(memberRoleName))
        {
          userIsMember = true;
        }
      }

      return userIsMember;
    }
  }
}
