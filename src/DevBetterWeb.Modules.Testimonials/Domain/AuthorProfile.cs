using Ardalis.GuardClauses;
using DevBetterWeb.Modules.Testimonials.Contracts;

namespace DevBetterWeb.Modules.Testimonials.Domain;

/// <summary>
/// How the member wants to be known alongside their testimonial.
/// </summary>
internal sealed class AuthorProfile
{
	public const int MaxDisplayNameLength = TestimonialLimits.MaxDisplayNameLength;
	public const int MaxTitleLength = TestimonialLimits.MaxTitleLength;
	public const int MaxOrganizationLength = TestimonialLimits.MaxOrganizationLength;
	public const int MaxPhotoUrlLength = TestimonialLimits.MaxPhotoUrlLength;

	// EF Core
	private AuthorProfile()
	{
		DisplayName = string.Empty;
	}

	private AuthorProfile(string displayName, string? title, string? organization, string? photoUrl)
	{
		DisplayName = displayName;
		Title = title;
		Organization = organization;
		PhotoUrl = photoUrl;
	}

	public string DisplayName { get; private set; }
	public string? Title { get; private set; }
	public string? Organization { get; private set; }

	/// <summary>
	/// Optional absolute https URL overriding the member's avatar. Null means use the avatar.
	/// </summary>
	public string? PhotoUrl { get; private set; }

	public static AuthorProfile Create(string displayName, string? title = null, string? organization = null, string? photoUrl = null)
	{
		Guard.Against.NullOrWhiteSpace(displayName);
		string name = displayName.Trim();
		Guard.Against.StringTooLong(name, MaxDisplayNameLength);

		string? normalizedTitle = Normalize(title, MaxTitleLength, nameof(title));
		string? normalizedOrganization = Normalize(organization, MaxOrganizationLength, nameof(organization));
		string? normalizedPhotoUrl = Normalize(photoUrl, MaxPhotoUrlLength, nameof(photoUrl));

		if (normalizedPhotoUrl is not null && !IsHttpsUrl(normalizedPhotoUrl))
		{
			throw new ArgumentException("Photo URL must be an absolute https URL.", nameof(photoUrl));
		}

		return new AuthorProfile(name, normalizedTitle, normalizedOrganization, normalizedPhotoUrl);
	}

	private static string? Normalize(string? value, int maxLength, string parameterName)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}

		string trimmed = value.Trim();
		Guard.Against.StringTooLong(trimmed, maxLength, parameterName);
		return trimmed;
	}

	// https only: avoids mixed content on the site and rules out javascript:/data: schemes.
	internal static bool IsHttpsUrl(string value) =>
		Uri.TryCreate(value, UriKind.Absolute, out Uri? uri)
		&& uri.Scheme == Uri.UriSchemeHttps;
}
