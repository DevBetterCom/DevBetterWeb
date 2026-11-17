namespace DevBetterWeb.Modules.Testimonials.Contracts;

/// <summary>
/// What a member enters when writing or editing a testimonial.
/// </summary>
/// <param name="PhotoUrl">Optional absolute https URL; null or blank falls back to the member's avatar.</param>
public sealed record TestimonialDraftInput(
	string Content,
	string DisplayName,
	string? Title,
	string? Organization,
	string? PhotoUrl);
