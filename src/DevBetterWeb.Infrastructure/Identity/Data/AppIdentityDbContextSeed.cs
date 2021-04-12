using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Web.Areas.Identity
{
  public class AppIdentityDbContextSeed
  {
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
      await roleManager.CreateAsync(new IdentityRole(AuthConstants.Roles.ADMINISTRATORS));
      await roleManager.CreateAsync(new IdentityRole(AuthConstants.Roles.MEMBERS));
      await roleManager.CreateAsync(new IdentityRole(AuthConstants.Roles.ALUMNI));

      var defaultUser = await CreateUser(userManager, "demouser@microsoft.com");
      await userManager.AddToRoleAsync(defaultUser, AuthConstants.Roles.MEMBERS);

      var adminUser = await CreateUser(userManager, "admin@test.com");
      await userManager.AddToRoleAsync(adminUser, AuthConstants.Roles.ADMINISTRATORS);

      var alumniUser = await CreateUser(userManager, "alumni@test.com");
      await userManager.AddToRoleAsync(alumniUser, AuthConstants.Roles.ALUMNI);
    }

    private static async Task<ApplicationUser> CreateUser(UserManager<ApplicationUser> userManager,
      string userName)
    {
      var user = new ApplicationUser { UserName = userName, Email = userName, EmailConfirmed = true };
      await userManager.CreateAsync(user, AuthConstants.DEFAULT_PASSWORD);
      return await userManager.FindByNameAsync(userName);
    }
  }
}
