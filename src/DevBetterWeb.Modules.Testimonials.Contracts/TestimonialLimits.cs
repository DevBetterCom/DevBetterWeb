namespace DevBetterWeb.Modules.Testimonials.Contracts;

/// <summary>
/// Field limits for testimonials, shared with callers so their forms can validate up front.
/// </summary>
public static class TestimonialLimits
{
	public const int MaxContentLength = 4000;
	public const int MaxFeedbackLength = 2000;
	public const int MaxDisplayNameLength = 100;
	public const int MaxTitleLength = 100;
	public const int MaxOrganizationLength = 100;
	public const int MaxPhotoUrlLength = 500;
}
