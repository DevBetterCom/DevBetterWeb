using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core;
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

    var defaultUser = await CreateUser(userManager, AuthConstants.Users.Demo.EMAIL);
    await userManager.AddToRoleAsync(defaultUser, AuthConstants.Roles.MEMBERS);

    var defaultUser2 = await CreateUser(userManager, AuthConstants.Users.Demo2.EMAIL);
    await userManager.AddToRoleAsync(defaultUser2, AuthConstants.Roles.MEMBERS);

    var defaultUser3 = await CreateUser(userManager, AuthConstants.Users.Demo3.EMAIL);
    await userManager.AddToRoleAsync(defaultUser3, AuthConstants.Roles.MEMBERS);

    var defaultUser4 = await CreateUser(userManager, AuthConstants.Users.Demo4.EMAIL);
    await userManager.AddToRoleAsync(defaultUser4, AuthConstants.Roles.MEMBERS);
    
    var nonMember = await CreateUser(userManager, AuthConstants.Users.NonMember.EMAIL);
    await userManager.AddToRoleAsync(nonMember, AuthConstants.Roles.MEMBERS);

    var adminUser = await CreateUser(userManager, AuthConstants.Users.Admin.EMAIL);
    await userManager.AddToRoleAsync(adminUser, AuthConstants.Roles.ADMINISTRATORS);

    var alumniUser = await CreateUser(userManager, AuthConstants.Users.Alumni.EMAIL);
    await userManager.AddToRoleAsync(alumniUser, AuthConstants.Roles.ALUMNI);

    var alumniUser2 = await CreateUser(userManager, AuthConstants.Users.Alumni2.EMAIL);
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
