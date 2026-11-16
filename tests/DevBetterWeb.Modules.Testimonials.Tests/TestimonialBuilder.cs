using DevBetterWeb.Modules.Testimonials.Contracts;
using DevBetterWeb.Modules.Testimonials.Domain;

namespace DevBetterWeb.Modules.Testimonials.Tests;

internal static class TestimonialBuilder
{
	public const int MemberId = 42;
	public const string DefaultContent = "devBetter changed my career.";
	public static readonly DateTimeOffset Created = new(2026, 11, 1, 12, 0, 0, TimeSpan.Zero);

	public static AuthorProfile Author(string name = "Ada Lovelace") =>
		AuthorProfile.Create(name, "Engineer", "Analytical Engines Ltd", "https://example.com/ada.png");

	public static Testimonial Draft(int memberId = MemberId) =>
		Testimonial.CreateDraft(memberId, DefaultContent, Author(), Created);

	/// <summary>
	/// Builds a testimonial in the given status by walking the legal transitions.
	/// </summary>
	public static Testimonial InStatus(TestimonialStatus status, DateTimeOffset? at = null, int memberId = MemberId)
	{
		var when = at ?? Created.AddDays(1);
		var testimonial = Draft(memberId);
		switch (status)
		{
			case TestimonialStatus.Draft:
				break;
			case TestimonialStatus.PendingReview:
				testimonial.Submit(when);
				break;
			case TestimonialStatus.ChangesRequested:
				testimonial.Submit(when);
				testimonial.RequestChanges("Please shorten it.", when);
				break;
			case TestimonialStatus.Accepted:
				testimonial.Submit(when);
				testimonial.Accept(when);
				break;
			case TestimonialStatus.Declined:
				testimonial.Submit(when);
				testimonial.Decline(when);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(status), status, null);
		}

		return testimonial;
	}
}
