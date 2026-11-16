using DevBetterWeb.Modules.Testimonials.Contracts;
using DevBetterWeb.Modules.Testimonials.Domain;

namespace DevBetterWeb.Modules.Testimonials.Application;

/// <summary>
/// Turns author input problems into field-level messages before the domain's guard clauses
/// (which remain the last line of defense) would throw.
/// </summary>
internal static class TestimonialDraftInputValidator
{
	public static IReadOnlyList<TestimonialValidationError> Validate(TestimonialDraftInput? input)
	{
		if (input is null)
		{
			return [new TestimonialValidationError(null, "A testimonial is required.")];
		}

		var errors = new List<TestimonialValidationError>();

		Required(errors, input.Content, nameof(TestimonialDraftInput.Content), "testimonial", TestimonialLimits.MaxContentLength);
		Required(errors, input.DisplayName, nameof(TestimonialDraftInput.DisplayName), "display name", TestimonialLimits.MaxDisplayNameLength);
		Optional(errors, input.Title, nameof(TestimonialDraftInput.Title), "title", TestimonialLimits.MaxTitleLength);
		Optional(errors, input.Organization, nameof(TestimonialDraftInput.Organization), "company or organization", TestimonialLimits.MaxOrganizationLength);

		if (Optional(errors, input.PhotoUrl, nameof(TestimonialDraftInput.PhotoUrl), "photo URL", TestimonialLimits.MaxPhotoUrlLength) is string photoUrl
			&& !AuthorProfile.IsHttpsUrl(photoUrl))
		{
			errors.Add(new TestimonialValidationError(nameof(TestimonialDraftInput.PhotoUrl), "Photo URL must be an absolute https:// URL."));
		}

		return errors;
	}

	private static void Required(List<TestimonialValidationError> errors, string? value, string field, string label, int maxLength)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			errors.Add(new TestimonialValidationError(field, $"The {label} is required."));
			return;
		}

		CheckLength(errors, value.Trim(), field, label, maxLength);
	}

	private static string? Optional(List<TestimonialValidationError> errors, string? value, string field, string label, int maxLength)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}

		string trimmed = value.Trim();
		return CheckLength(errors, trimmed, field, label, maxLength) ? trimmed : null;
	}

	private static bool CheckLength(List<TestimonialValidationError> errors, string trimmed, string field, string label, int maxLength)
	{
		if (trimmed.Length > maxLength)
		{
			errors.Add(new TestimonialValidationError(field, $"The {label} must be at most {maxLength} characters."));
			return false;
		}

		return true;
	}
}
