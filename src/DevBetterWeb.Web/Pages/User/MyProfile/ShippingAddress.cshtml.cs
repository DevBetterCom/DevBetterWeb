using System.Security.Claims;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class ShippingAddressModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IMemberRegistrationService _memberRegistrationService;
	private readonly IRepository<Member> _memberRepository;

	public ShippingAddressModel(UserManager<ApplicationUser> userManager,
		IMemberRegistrationService memberRegistrationService,
		IRepository<Member> memberRepository)
	{
		_userManager = userManager;
		_memberRegistrationService = memberRegistrationService;
		_memberRepository = memberRepository;
	}

	[BindProperty]
	public UserShippingAddressUpdateModel UserShippingAddressUpdateModel { get; set; } = null!;

	public async Task OnGetAsync(string? userId = null)
	{
		var (member, actualUserId) = await GetMemberAsync(User, userId);
		if (member is null && actualUserId is null) return;
		if (member == null)
		{
			member = await _memberRegistrationService.RegisterMemberAsync(actualUserId!);
		}

		UserShippingAddressUpdateModel = new UserShippingAddressUpdateModel(member);
	}

	public async Task OnPost(string? userId = null)
	{
		if (!ModelState.IsValid) return;

		var (member, actualUserId) = await GetMemberAsync(User, userId);
		if (member is null && actualUserId is null) return;
		if (member is null) throw new MemberNotFoundException(actualUserId!);

		member.UpdateShippingAddress(
			UserShippingAddressUpdateModel.Street,
			UserShippingAddressUpdateModel.City,
			UserShippingAddressUpdateModel.State,
			UserShippingAddressUpdateModel.PostalCode,
			UserShippingAddressUpdateModel.Country);
		member.UpdateAddress(member.ShippingAddress!.ToString());

		await _memberRepository.UpdateAsync(member);
	}

	private async Task<(Member? member, string? userId)> GetMemberAsync(ClaimsPrincipal user, string? userId = null)
	{
		var isAdmin = user.IsInRole(AuthConstants.Roles.ADMINISTRATORS);

		if (isAdmin && !string.IsNullOrEmpty(userId))
		{
			var spec = new MemberByUserIdSpec(userId);
			var member = await _memberRepository.FirstOrDefaultAsync(spec);
			return (member, userId);
		}

		var currentUserName = user.Identity?.Name;
		if (currentUserName == null)
		{
			return (null, null);
		}

		var applicationUser = await _userManager.FindByNameAsync(currentUserName);
		if (applicationUser == null)
		{
			return (null, null);
		}

		var userSpec = new MemberByUserIdSpec(applicationUser.Id);
		var currentUserMember = await _memberRepository.FirstOrDefaultAsync(userSpec);

		return (currentUserMember, applicationUser.Id);
	}
}
