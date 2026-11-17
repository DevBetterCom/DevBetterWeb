using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Modules.Testimonials.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.User.MyProfile.Testimonials;

public class EditModel : MyTestimonialsPageModel
{
	public EditModel(
		ITestimonialAuthoring authoring,
		UserManager<ApplicationUser> userManager,
		IRepository<Member> memberRepository,
		IMemberRegistrationService memberRegistrationService)
		: base(authoring, userManager, memberRepository, memberRegistrationService)
	{
	}

	[BindProperty]
	public TestimonialInputModel Input { get; set; } = new();

	/// <summary>The testimonial being edited; null when writing a new one.</summary>
	public MemberTestimonialDto? Testimonial { get; private set; }

	public bool IsNew => Testimonial is null;
	public bool IsReadOnly => Testimonial is { CanEdit: false };

	public async Task<IActionResult> OnGetAsync(int? id)
	{
		var member = await GetCurrentMemberAsync();

		if (id is not int testimonialId)
		{
			Input = new TestimonialInputModel { DisplayName = DefaultDisplayName(member) };
			return Page();
		}

		Testimonial = await Authoring.GetForMemberAsync(member.Id, testimonialId, HttpContext.RequestAborted);
		if (Testimonial is null)
		{
			return NotFound();
		}

		Input = TestimonialInputModel.From(Testimonial);
		return Page();
	}

	public Task<IActionResult> OnPostAsync(int? id) => SaveAsync(id, submitForReview: false);

	public Task<IActionResult> OnPostSubmitForReviewAsync(int? id) => SaveAsync(id, submitForReview: true);

	private async Task<IActionResult> SaveAsync(int? id, bool submitForReview)
	{
		var member = await GetCurrentMemberAsync();
		var cancellationToken = HttpContext.RequestAborted;

		if (id is int existingId)
		{
			Testimonial = await Authoring.GetForMemberAsync(member.Id, existingId, cancellationToken);
			if (Testimonial is null)
			{
				return NotFound();
			}

			if (!Testimonial.CanEdit)
			{
				return ConflictPage($"You can't change a testimonial while it is {TestimonialDisplay.Label(Testimonial.Status).ToLowerInvariant()}.");
			}
		}

		if (!ModelState.IsValid)
		{
			return Page();
		}

		var input = Input.ToDraftInput();
		var saved = Testimonial is null
			? await Authoring.CreateDraftAsync(member.Id, input, cancellationToken)
			: await Authoring.UpdateAsync(member.Id, Testimonial.Id, input, cancellationToken);

		if (!saved.IsSuccess)
		{
			return await FailureAsync(member.Id, id, saved);
		}

		int savedId = saved.TestimonialId!.Value;
		if (!submitForReview)
		{
			StatusMessage = "Your testimonial was saved.";
			return RedirectToPage("./Index");
		}

		var submitted = await Authoring.SubmitAsync(member.Id, savedId, cancellationToken);
		if (!submitted.IsSuccess)
		{
			ErrorMessage = "Your changes were saved, but the testimonial could not be submitted. " + string.Join(" ", ErrorMessages(submitted));
			return RedirectToPage("./Edit", new { id = savedId });
		}

		StatusMessage = "Your testimonial was submitted for review.";
		return RedirectToPage("./Index");
	}

	private async Task<IActionResult> FailureAsync(int memberId, int? id, TestimonialOperationResult result)
	{
		switch (result.Status)
		{
			case TestimonialOperationStatus.NotFound:
				return NotFound();

			case TestimonialOperationStatus.Invalid:
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(error.Field is null ? string.Empty : $"{nameof(Input)}.{error.Field}", error.Message);
				}
				return Page();

			default:
				// Status changed since the page loaded (e.g. an admin reviewed it, or it was submitted in another tab).
				if (id is int existingId)
				{
					Testimonial = await Authoring.GetForMemberAsync(memberId, existingId, HttpContext.RequestAborted);
				}
				return ConflictPage(string.Join(" ", ErrorMessages(result)));
		}
	}

	private PageResult ConflictPage(string message)
	{
		ModelState.AddModelError(string.Empty, message);
		var page = Page();
		page.StatusCode = StatusCodes.Status409Conflict;
		return page;
	}

	private static IEnumerable<string> ErrorMessages(TestimonialOperationResult result) =>
		result.Errors.Select(e => e.Message);

	private static string DefaultDisplayName(Member member) =>
		string.IsNullOrWhiteSpace(member.FirstName) || string.IsNullOrWhiteSpace(member.LastName)
			? string.Empty
			: $"{member.FirstName} {member.LastName}";
}
