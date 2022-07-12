using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
public class IndexModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	public readonly IMemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;
	private readonly IRepository<Member> _memberRepository;

	public List<MemberLinksDTO> Members { get; set; } = new List<MemberLinksDTO>();
	public List<MemberSubscriptionPercentCircleViewModel> PercentModels { get; set; } = new List<MemberSubscriptionPercentCircleViewModel>();
	public bool IsAdministrator { get; set; }

	public IndexModel(UserManager<ApplicationUser> userManager, IMemberSubscriptionPeriodCalculationsService memberSubscriptionPeriodCalculationsService, 
		IRepository<Member> memberRepository)
	{
		_userManager = userManager;
		_memberSubscriptionPeriodCalculationsService = memberSubscriptionPeriodCalculationsService;
		_memberRepository = memberRepository;
	}

	public async Task OnGet()
	{
		IsAdministrator = User.IsInRole(AuthConstants.Roles.ADMINISTRATORS);

		IList<ApplicationUser> users;

		if (IsAdministrator)
		{
			users = _userManager.Users.ToList();
		}
		else
		{
			users = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);
		}

		var members = await GetMembersByUsers(users);

		Members = members.Select(MemberLinksDTO.FromMemberEntity)
				.ToList();

		await UpdateMembersPercentageProgressToAlumniAndMemberStatus();
	}

	private async Task UpdateMembersPercentageProgressToAlumniAndMemberStatus()
	{
		var memberUserIds = await GetUserIdsByMemberRole();
		foreach (var member in Members)
		{
			var model = new MemberSubscriptionPercentCircleViewModel(0);
			model.Percentage = _memberSubscriptionPeriodCalculationsService.GetPercentageProgressToAlumniStatus(member.SubscribedDays);
			PercentModels.Add(model);

			if (memberUserIds.Contains(member.UserId!))
			{
				member.IsMember = true;
			}
			else
			{
				member.IsMember = false;
			}
		}
	}

	private async Task<List<Member>> GetMembersByUsers(IList<ApplicationUser> users)
	{
		// TODO: Write a LINQ join for this
		var userIds = users.Select(x => x.Id).ToList();
		var membersByUsersIdSpec = new MembersByUsersIdSpec(userIds);
		var members = await _memberRepository.ListAsync(membersByUsersIdSpec);

		return members;
	}

	private async Task<List<string>> GetUserIdsByMemberRole()
	{
		IList<ApplicationUser> memberUsers = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);
		var memberUserIds = memberUsers.Select(x => x.Id).ToList();

		return memberUserIds;
	}
}
