using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Modules.Testimonials.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.Pages.User.MyProfile.Testimonials;

public class IndexModel : MyTestimonialsPageModel
{
	public IndexModel(
		ITestimonialAuthoring authoring,
		UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository,
		IMemberRegistrationService memberRegistrationService)
		: base(authoring, userManager, memberRepository, memberRegistrationService)
	{
	}

	public IReadOnlyList<MemberTestimonialDto> Testimonials { get; private set; } = [];

	public async Task OnGetAsync()
	{
		var member = await GetCurrentMemberAsync();
		Testimonials = await Authoring.ListForMemberAsync(member.Id, HttpContext.RequestAborted);
	}

	public async Task<IActionResult> OnPostSubmitAsync(int id)
	{
		var member = await GetCurrentMemberAsync();
		var result = await Authoring.SubmitAsync(member.Id, id, HttpContext.RequestAborted);
		return ToResponse(result, "Your testimonial was submitted for review.");
	}

	public async Task<IActionResult> OnPostDeleteAsync(int id)
	{
		var member = await GetCurrentMemberAsync();
		var result = await Authoring.DeleteDraftAsync(member.Id, id, HttpContext.RequestAborted);
		return ToResponse(result, "Your draft was deleted.");
	}

	private IActionResult ToResponse(TestimonialOperationResult result, string successMessage)
	{
		switch (result.Status)
		{
			case TestimonialOperationStatus.NotFound:
				return NotFound();
			case TestimonialOperationStatus.Success:
				StatusMessage = successMessage;
				break;
			default:
				ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Message));
				break;
		}

		return RedirectToPage();
	}
}
