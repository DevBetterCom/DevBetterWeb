namespace DevBetterWeb.Modules.Testimonials.Contracts;

/// <summary>
/// A testimonial as seen by its author, including review status and feedback.
/// </summary>
public sealed record MemberTestimonialDto(
	int Id,
	string Content,
	string DisplayName,
	string? Title,
	string? Organization,
	string? PhotoUrl,
	TestimonialStatus Status,
	string? ReviewerFeedback,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	DateTimeOffset? SubmittedAt,
	DateTimeOffset? ReviewedAt)
{
	public bool CanEdit => Status is TestimonialStatus.Draft or TestimonialStatus.ChangesRequested;
	public bool CanSubmit => CanEdit;
	public bool CanDelete => Status == TestimonialStatus.Draft;
}
