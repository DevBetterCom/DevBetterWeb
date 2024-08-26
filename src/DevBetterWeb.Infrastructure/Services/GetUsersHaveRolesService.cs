using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Services;

public class GetUsersHaveRolesService : IGetUsersHaveRolesService
{
	private readonly UserManager<IdentityUser> _userManager;

	public GetUsersHaveRolesService(UserManager<IdentityUser> userManager)
	{
		_userManager = userManager;
	}

	public async Task<List<IdentityUser>> ExecuteAsync()
	{
		var usersWithRoles = new List<IdentityUser>();

		var allUsers = _userManager.Users.ToList();
		foreach (var user in allUsers)
		{
			if (user.UserName == null)
			{
				continue;
			}
			var roles = await _userManager.GetRolesAsync(user);

			if (roles.Any())
			{
				usersWithRoles.Add(user);
			}
		}

		return usersWithRoles;
	}
}
