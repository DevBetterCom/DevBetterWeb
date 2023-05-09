using DevBetterWeb.Core.Specs;
using DevBetterWeb.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Infrastructure.Interfaces;
using DevBetterWeb.Web.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Web.Services;

/// <summary>
/// Service class for handling operations related to members.
/// </summary>
public class MemberService : IMemberService
{
	/// <summary>
	/// The UserManager for managing users in the application.
	/// </summary>
	private readonly UserManager<ApplicationUser> _userManager;

	/// <summary>
	/// The repository for managing members in the database.
	/// </summary>
	private readonly IRepository<Member> _memberRepository;

	/// <summary>
	/// The service for managing non-current members.
	/// </summary>
	private readonly INonCurrentMembersService _nonCurrentMembersService;

	/// <summary>
	/// Initializes a new instance of the <see cref="MemberService"/> class.
	/// </summary>
	/// <param name="userManager">The UserManager for managing users in the application.</param>
	/// <param name="memberRepository">The repository for managing members in the database.</param>
	/// <param name="nonCurrentMembersService">The service for managing non-current members.</param>
	public MemberService(UserManager<ApplicationUser> userManager, IRepository<Member> memberRepository, INonCurrentMembersService nonCurrentMembersService)
	{
		_userManager = userManager;
		_memberRepository = memberRepository;
		_nonCurrentMembersService = nonCurrentMembersService;
	}

	/// <summary>
	/// Gets a list of all alumni members asynchronously.
	/// </summary>
	/// <returns>A list of all alumni members.</returns>
	public async Task<List<Member>> GetAlumniMembersAsync()
	{
		var usersInAlumniRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ALUMNI);
		var alumniUserIds = usersInAlumniRole.Select(x => x.Id).ToList();

		var alumniSpec = new MembersHavingUserIdsWithBooksSpec(alumniUserIds);
		var alumniMembers = await _memberRepository.ListAsync(alumniSpec);

		return alumniMembers;
	}

	/// <summary>
	/// Gets a list of all active alumni members asynchronously.
	/// </summary>
	/// <returns>A list of all active alumni members.</returns>
	public async Task<List<Member>> GetActiveAlumniMembersAsync()
	{
		var alumniMembers = await GetAlumniMembersAsync();
		return await _nonCurrentMembersService.RemoveNonCurrentMembersAsync(alumniMembers);
	}

	/// <summary>
	/// Gets a list of all members asynchronously.
	/// </summary>
	/// <returns>A list of all members.</returns>
	public async Task<List<Member>> GetMembersAsync()
	{
		var usersInMemberRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);
		var memberUserIds = usersInMemberRole.Select(x => x.Id).ToList();

		var memberSpec = new MembersHavingUserIdsWithBooksSpec(memberUserIds);
		var members = await _memberRepository.ListAsync(memberSpec);

		return members;
	}

	/// <summary>
	/// Gets a list of all active members asynchronously.
	/// </summary>
	/// <returns>A list of all active members.</returns>
	public async Task<List<Member>> GetActiveMembersAsync()
	{
		var members = await GetMembersAsync();
		return await _nonCurrentMembersService.RemoveNonCurrentMembersAsync(members);
	}
}
