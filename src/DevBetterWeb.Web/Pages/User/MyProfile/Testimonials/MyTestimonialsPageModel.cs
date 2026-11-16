using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Modules.Testimonials.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User.MyProfile.Testimonials;

/// <summary>
/// Shared plumbing for the member's testimonial pages: resolves the signed-in member, whose id
/// scopes every call into the Testimonials module.
/// </summary>
[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public abstract class MyTestimonialsPageModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRepository<Member> _memberRepository;
	private readonly IMemberRegistrationService _memberRegistrationService;

	protected MyTestimonialsPageModel(
		ITestimonialAuthoring authoring,
		UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository,
		IMemberRegistrationService memberRegistrationService)
	{
		Authoring = authoring;
		_userManager = userManager;
		_memberRepository = memberRepository;
		_memberRegistrationService = memberRegistrationService;
	}

	protected ITestimonialAuthoring Authoring { get; }

	[TempData]
	public string? StatusMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	protected async Task<Member> GetCurrentMemberAsync()
	{
		var userId = _userManager.GetUserId(User);
		if (string.IsNullOrEmpty(userId))
		{
			var applicationUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
			userId = applicationUser!.Id;
		}

		var member = await _memberRepository.FirstOrDefaultAsync(new MemberByUserIdSpec(userId), HttpContext.RequestAborted);
		return member ?? await _memberRegistrationService.RegisterMemberAsync(userId);
	}
}
