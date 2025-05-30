﻿using System.Threading.Tasks;
using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Infrastructure.Services;

public class AspNetCoreIdentityUserRoleMembershipService : IUserRoleMembershipService
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly IAppLogger<AspNetCoreIdentityUserEmailConfirmationService> _logger;
	private readonly IMediator _mediator;

	public AspNetCoreIdentityUserRoleMembershipService(UserManager<ApplicationUser> userManager,
      RoleManager<IdentityRole> roleManager,
      IAppLogger<AspNetCoreIdentityUserEmailConfirmationService> logger,
			IMediator mediator)
  {
    _userManager = userManager;
    _roleManager = roleManager;
    _logger = logger;
		_mediator = mediator;
	}

  public async Task AddUserToRoleAsync(string userId, string roleId)
  {
    var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
    if (user == null) throw new UserNotFoundException(userId);

    var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
    if (role == null) throw new RoleNotFoundException(roleId);

    await _userManager.AddToRoleAsync(user, role.Name!);

    var userAddedToRoleEvent = new UserAddedToRoleEvent(user!.Email!, role.Name!);
    await _mediator.Publish(userAddedToRoleEvent);
  }

  public async Task AddUserToRoleByRoleNameAsync(string userId, string roleName)
  {
    var role = await _roleManager.FindByNameAsync(roleName);
		if (role is null) throw new RoleNotFoundException(roleName);
    var roleId = role.Id;

    await AddUserToRoleAsync(userId, roleId);
  }

  public async Task RemoveUserFromRoleAsync(string userId, string roleId)
  {
    var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
    if (user == null) throw new UserNotFoundException(userId);

    var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
    if (role == null) throw new RoleNotFoundException(roleId);

    // check if user is in role?
    if (await _userManager.IsInRoleAsync(user, role.Name!))
    {
      await _userManager.RemoveFromRoleAsync(user, role!.Name!);

      var userRemovedFromRoleEvent = new UserRemovedFromRoleEvent(user!.Email!, role.Name!);
      await _mediator.Publish(userRemovedFromRoleEvent);
    }
    else
    {
      _logger.LogWarning($"Attempted to remove {user.UserName} from role {role.Name} they weren't in.");
    }
  }

  public async Task RemoveUserFromRoleByRoleNameAsync(string userId, string roleName)
  {
    var role = await _roleManager.FindByNameAsync(roleName);
		if (role is null) throw new RoleNotFoundException(roleName);
    var roleId = role.Id;

    await RemoveUserFromRoleAsync(userId, roleId);
  }
}
