using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Areas.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Constants.Roles.ADMINISTRATORS));
            await roleManager.CreateAsync(new IdentityRole(Constants.Roles.MEMBERS));

            string defaultUserName = "demouser@microsoft.com";
            string defaultPassword = "Pass@word1";

            var defaultUser = new ApplicationUser { UserName = defaultUserName, Email = defaultUserName };
            await userManager.CreateAsync(defaultUser, "Pass@word1");
            defaultUser = await userManager.FindByNameAsync(defaultUserName);
            await userManager.AddToRoleAsync(defaultUser, Constants.Roles.MEMBERS);

            string adminUserName = "admin@test.com";
            var adminUser = new ApplicationUser { UserName = adminUserName, Email = adminUserName };
            await userManager.CreateAsync(adminUser, defaultPassword);
            adminUser = await userManager.FindByNameAsync(adminUserName);

            await userManager.AddToRoleAsync(adminUser, Constants.Roles.ADMINISTRATORS);
        }
    }
}
