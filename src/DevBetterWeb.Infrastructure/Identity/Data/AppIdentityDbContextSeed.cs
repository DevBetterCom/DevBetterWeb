﻿using DevBetterWeb.Web.Areas.Identity.Data;
using System.Threading.Tasks;
using DevBetterWeb.Core;
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

            string defaultUserName = "demouser@microsoft.com";
            string defaultPassword = AuthConstants.DEFAULT_PASSWORD;

            var defaultUser = new ApplicationUser { UserName = defaultUserName, Email = defaultUserName, EmailConfirmed = true };
            await userManager.CreateAsync(defaultUser, AuthConstants.DEFAULT_PASSWORD);
            defaultUser = await userManager.FindByNameAsync(defaultUserName);
            await userManager.AddToRoleAsync(defaultUser, AuthConstants.Roles.MEMBERS);

            string adminUserName = "admin@test.com";
            var adminUser = new ApplicationUser { UserName = adminUserName, Email = adminUserName, EmailConfirmed = true };
            await userManager.CreateAsync(adminUser, defaultPassword);
            adminUser = await userManager.FindByNameAsync(adminUserName);

            await userManager.AddToRoleAsync(adminUser, AuthConstants.Roles.ADMINISTRATORS);
        }
    }
}
