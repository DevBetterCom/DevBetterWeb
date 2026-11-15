namespace DevBetterWeb.Modules.Testimonials.Contracts;

/// <summary>
/// Lifecycle of a member testimonial.
/// Draft -> PendingReview -> Accepted | Declined | ChangesRequested.
/// ChangesRequested -> PendingReview. Accepted -> Declined (unpublish). Declined is terminal.
/// </summary>
public enum TestimonialStatus
{
	Draft = 0,
	PendingReview = 1,
	ChangesRequested = 2,
	Accepted = 3,
	Declined = 4,
}
