using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Services;

/// <summary>
/// Provides functionality to manage non-current members.
/// </summary>
public class NonCurrentMembersService : INonCurrentMembersService
{
	private readonly IRepository<Member> _memberRepository;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;

	/// <summary>
	/// Initializes a new instance of the <see cref="NonCurrentMembersService"/> class.
	/// </summary>
	/// <param name="memberRepository">The repository for accessing member data.</param>
	/// <param name="userManager">The user manager for managing application users.</param>
	/// <param name="roleManager">The role manager for managing roles in the application.</param>
	public NonCurrentMembersService(IRepository<Member> memberRepository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
	{
		_memberRepository = memberRepository;
		_userManager = userManager;
		_roleManager = roleManager;
	}

	/// <summary>
	/// Retrieves a list of user IDs without member, alumni, and administrator roles.
	/// </summary>
	/// <returns>A list of user IDs without the specified roles.</returns>
	public async Task<List<string>> GetUsersIdsWithoutRolesAsync()
	{
		var memberRole = await _roleManager.FindByNameAsync(AuthConstants.Roles.MEMBERS);
		if (memberRole == null)
		{
			throw new ArgumentException($"Role with name {AuthConstants.Roles.MEMBERS} not found.");
		}

		var alumniRole = await _roleManager.FindByNameAsync(AuthConstants.Roles.ALUMNI);
		if (alumniRole == null)
		{
			throw new ArgumentException($"Role with name {AuthConstants.Roles.ALUMNI} not found.");
		}

		var adminRole = await _roleManager.FindByNameAsync(AuthConstants.Roles.ADMINISTRATORS);
		if (adminRole == null)
		{
			throw new ArgumentException($"Role with name {AuthConstants.Roles.ADMINISTRATORS} not found.");
		}

		var usersWithMemberRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);
		var usersWithAlumniRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ALUMNI);
		var usersWithAdminRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ADMINISTRATORS);
		var usersWithRoles = usersWithMemberRole.Concat(usersWithAlumniRole).Concat(usersWithAdminRole).Distinct();

		var allUsers = _userManager.Users.ToList();
		var usersWithoutRoles = allUsers.Except(usersWithRoles).ToList();

		return usersWithoutRoles.Select(u => u.Id).ToList();
	}

	/// <summary>
	/// Retrieves a list of non-current member IDs based on the provided list of user IDs without a member role.
	/// </summary>
	/// <param name="usersIdWithoutMemberRole">A list of user IDs without a member role.</param>
	/// <param name="cancellationToken">An optional token to cancel the operation.</param>
	/// <returns>A list of non-current member IDs.</returns>
	public async Task<List<int>> GetNonCurrentMembersAsync(List<string> usersIdWithoutMemberRole, CancellationToken cancellationToken = default)
	{
		var membersNonSubscriptionSpec = new MembersNonSubscriptionSpec(usersIdWithoutMemberRole);
		var nonMembers = await _memberRepository.ListAsync(membersNonSubscriptionSpec, cancellationToken);

		return nonMembers.Select(member => member.Id).ToList();
	}

	/// <summary>
	/// Removes non-current members from the provided list.
	/// </summary>
	/// <param name="members">The list of members to filter.</param>
	/// <returns>A filtered list of members that only includes current members.</returns>
	/// <remarks>
	/// This method first retrieves the IDs of non-current users from the `_nonCurrentMembersService`.
	/// Then, it retrieves the IDs of non-current members.
	/// Finally, it filters out any members from the provided list whose IDs are in the non-current members list.
	/// </remarks>
	public async Task<List<Member>> RemoveNonCurrentMembersAsync(List<Member> members)
	{
		var nonUsersId = await GetUsersIdsWithoutRolesAsync();
		var nonMembersId = await GetNonCurrentMembersAsync(nonUsersId);

		return members.Where(member => !nonMembersId.Contains(member.Id)).ToList();
	}
}
