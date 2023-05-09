using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Web.Areas.Identity;

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

    var defaultUser2 = await CreateUser(userManager, "demouser2@microsoft.com");
    await userManager.AddToRoleAsync(defaultUser2, AuthConstants.Roles.MEMBERS);

    var defaultUser3 = await CreateUser(userManager, "demouser3@microsoft.com");
    await userManager.AddToRoleAsync(defaultUser3, AuthConstants.Roles.MEMBERS);

    var defaultUser4 = await CreateUser(userManager, "demouser4@microsoft.com");
    await userManager.AddToRoleAsync(defaultUser4, AuthConstants.Roles.MEMBERS);
    
    var nonMember = await CreateUser(userManager, "non-member@microsoft.com");
    await userManager.AddToRoleAsync(nonMember, AuthConstants.Roles.MEMBERS);

    var adminUser = await CreateUser(userManager, "admin@test.com");
    await userManager.AddToRoleAsync(adminUser, AuthConstants.Roles.ADMINISTRATORS);

    var alumniUser = await CreateUser(userManager, "alumni@test.com");
    await userManager.AddToRoleAsync(alumniUser, AuthConstants.Roles.ALUMNI);

    var alumniUser2 = await CreateUser(userManager, "alumni2@test.com");
    await userManager.AddToRoleAsync(alumniUser2, AuthConstants.Roles.ALUMNI);
	}

  public static async Task<bool> RemoveUserFromRoleAsync(UserManager<ApplicationUser> userManager, string email, string roleName)
  {
	  var user = await userManager.FindByEmailAsync(email);
	  if (user == null)
	  {
		  return false;
	  }

		await userManager.RemoveFromRolesAsync(user, new List<string> { roleName });

		return true;
  }

  private static async Task<ApplicationUser> CreateUser(UserManager<ApplicationUser> userManager,
    string userName)
  {
    var user = new ApplicationUser { UserName = userName, Email = userName, EmailConfirmed = true, DateCreated = DateTime.UtcNow };
    await userManager.CreateAsync(user, AuthConstants.DEFAULT_PASSWORD);
		var newUser = await userManager.FindByNameAsync(userName!);
		return newUser!;
  }
}
