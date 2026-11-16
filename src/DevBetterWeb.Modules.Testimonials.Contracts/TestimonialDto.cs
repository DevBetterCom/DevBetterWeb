namespace DevBetterWeb.Modules.Testimonials.Contracts;

/// <summary>
/// Read model of a testimonial for display outside the module.
/// </summary>
/// <param name="PhotoUrl">Optional override; when null, callers fall back to the member's avatar.</param>
public sealed record TestimonialDto(
	int Id,
	int MemberId,
	string Content,
	string DisplayName,
	string? Title,
	string? Organization,
	string? PhotoUrl,
	TestimonialStatus Status,
	DateTimeOffset? ReviewedAt);
