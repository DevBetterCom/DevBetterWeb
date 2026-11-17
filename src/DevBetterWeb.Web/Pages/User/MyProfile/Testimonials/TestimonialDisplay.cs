using System.Linq;
using DevBetterWeb.Modules.Testimonials.Contracts;

namespace DevBetterWeb.Web.Pages.User.MyProfile.Testimonials;

public static class TestimonialDisplay
{
	public static string Label(TestimonialStatus status) => status switch
	{
		TestimonialStatus.Draft => "Draft",
		TestimonialStatus.PendingReview => "Pending review",
		TestimonialStatus.ChangesRequested => "Changes requested",
		TestimonialStatus.Accepted => "Accepted",
		TestimonialStatus.Declined => "Declined",
		_ => status.ToString(),
	};

	public static string BadgeClass(TestimonialStatus status) => status switch
	{
		TestimonialStatus.Draft => "badge badge-secondary",
		TestimonialStatus.PendingReview => "badge badge-info",
		TestimonialStatus.ChangesRequested => "badge badge-warning",
		TestimonialStatus.Accepted => "badge badge-success",
		TestimonialStatus.Declined => "badge badge-danger",
		_ => "badge badge-light",
	};

	public static string Explanation(TestimonialStatus status) => status switch
	{
		TestimonialStatus.Draft => "Only you can see this draft. Submit it when you're ready for an admin to review it.",
		TestimonialStatus.PendingReview => "An admin is reviewing this testimonial. You can't change it while it is pending review.",
		TestimonialStatus.ChangesRequested => "An admin asked for changes. Update it and submit it again.",
		TestimonialStatus.Accepted => "This testimonial is published on the site.",
		TestimonialStatus.Declined => "This testimonial was not accepted for publication.",
		_ => string.Empty,
	};

	/// <summary>"Name, Title, Organization", skipping blank parts.</summary>
	public static string AuthorLine(MemberTestimonialDto testimonial) =>
		string.Join(", ", new[] { testimonial.DisplayName, testimonial.Title, testimonial.Organization }
			.Where(part => !string.IsNullOrWhiteSpace(part)));
}
