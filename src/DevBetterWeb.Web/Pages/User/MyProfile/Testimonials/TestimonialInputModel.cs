using System.ComponentModel.DataAnnotations;
using DevBetterWeb.Modules.Testimonials.Contracts;

namespace DevBetterWeb.Web.Pages.User.MyProfile.Testimonials;

/// <summary>
/// Form model for writing a testimonial. The module re-validates everything server side.
/// </summary>
public class TestimonialInputModel
{
	[Required]
	[StringLength(TestimonialLimits.MaxContentLength)]
	[Display(Name = "Your testimonial")]
	public string Content { get; set; } = string.Empty;

	[Required]
	[StringLength(TestimonialLimits.MaxDisplayNameLength)]
	[Display(Name = "Display name")]
	public string DisplayName { get; set; } = string.Empty;

	[StringLength(TestimonialLimits.MaxTitleLength)]
	[Display(Name = "Title")]
	public string? Title { get; set; }

	[StringLength(TestimonialLimits.MaxOrganizationLength)]
	[Display(Name = "Company or organization")]
	public string? Organization { get; set; }

	[StringLength(TestimonialLimits.MaxPhotoUrlLength)]
	[Display(Name = "Photo URL")]
	public string? PhotoUrl { get; set; }

	public static TestimonialInputModel From(MemberTestimonialDto testimonial) => new()
	{
		Content = testimonial.Content,
		DisplayName = testimonial.DisplayName,
		Title = testimonial.Title,
		Organization = testimonial.Organization,
		PhotoUrl = testimonial.PhotoUrl,
	};

	public TestimonialDraftInput ToDraftInput() => new(Content, DisplayName, Title, Organization, PhotoUrl);
}
