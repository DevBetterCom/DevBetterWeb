using DevBetterWeb.Modules.Testimonials.Domain;
using Shouldly;
using Xunit;

namespace DevBetterWeb.Modules.Testimonials.Tests.Domain;

public class AuthorProfileTests
{
	[Fact]
	public void CreateTrimsValuesAndNormalizesBlanksToNull()
	{
		var author = AuthorProfile.Create("  Ada  ", "  ", "", null);

		author.DisplayName.ShouldBe("Ada");
		author.Title.ShouldBeNull();
		author.Organization.ShouldBeNull();
		author.PhotoUrl.ShouldBeNull();
	}

	[Fact]
	public void CreateKeepsAllProvidedValues()
	{
		var author = AuthorProfile.Create("Ada", "Engineer", "Analytical Engines", "https://example.com/a.png");

		author.DisplayName.ShouldBe("Ada");
		author.Title.ShouldBe("Engineer");
		author.Organization.ShouldBe("Analytical Engines");
		author.PhotoUrl.ShouldBe("https://example.com/a.png");
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void CreateRequiresDisplayName(string displayName)
	{
		Should.Throw<ArgumentException>(() => AuthorProfile.Create(displayName));
	}

	[Fact]
	public void CreateRejectsNullDisplayName()
	{
		Should.Throw<ArgumentNullException>(() => AuthorProfile.Create(null!));
	}

	[Fact]
	public void CreateRejectsDisplayNameOverMaxLength()
	{
		Should.Throw<ArgumentException>(() => AuthorProfile.Create(new string('a', AuthorProfile.MaxDisplayNameLength + 1)));
	}

	[Fact]
	public void CreateRejectsTitleOverMaxLength()
	{
		Should.Throw<ArgumentException>(() => AuthorProfile.Create("Ada", title: new string('a', AuthorProfile.MaxTitleLength + 1)));
	}

	[Fact]
	public void CreateRejectsOrganizationOverMaxLength()
	{
		Should.Throw<ArgumentException>(() => AuthorProfile.Create("Ada", organization: new string('a', AuthorProfile.MaxOrganizationLength + 1)));
	}

	[Fact]
	public void CreateRejectsPhotoUrlOverMaxLength()
	{
		string url = "https://example.com/" + new string('a', AuthorProfile.MaxPhotoUrlLength);

		Should.Throw<ArgumentException>(() => AuthorProfile.Create("Ada", photoUrl: url));
	}

	[Fact]
	public void CreateAcceptsHttpsPhotoUrl()
	{
		AuthorProfile.Create("Ada", photoUrl: "https://example.com/a.png").PhotoUrl.ShouldBe("https://example.com/a.png");
	}

	[Theory]
	[InlineData("http://example.com/a.png")]
	[InlineData("javascript:alert(1)")]
	[InlineData("data:image/png;base64,AAAA")]
	[InlineData("ftp://example.com/a.png")]
	[InlineData("/images/a.png")]
	[InlineData("not a url")]
	public void CreateRejectsNonHttpPhotoUrls(string url)
	{
		Should.Throw<ArgumentException>(() => AuthorProfile.Create("Ada", photoUrl: url));
	}
}
