using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Pages.User;

public class UserLinksUpdateModel
{

	[ValidUrlContainingString("LinkedIn")]
	public string? LinkedInUrl { get; set; }
	[ValidUrlContainingString("Twitter")]
	public string? TwitterUrl { get; set; }
	[ValidUrlContainingString("GitHub")]
	public string? GithubUrl { get; set; }
	[ValidUrl]
	public string? BlogUrl { get; set; }
	[ValidUrlContainingString("Twitch")]
	public string? TwitchUrl { get; set; }
	[ValidUrlContainingString("YouTube")]
	public string? YouTubeUrl { get; set; }
	[ValidUrl]
	public string? OtherUrl { get; set; }
	[ValidUrlContainingString("CodinGame")]
	public string? CodinGameUrl { get; set; }
	[ValidUrl]
	public string? MastodonUrl { get; set; }
	public string? UserId { get; set; }

	public UserLinksUpdateModel()
	{

	}

	public UserLinksUpdateModel(Member member)
	{

		UserId = member.UserId;
		BlogUrl = member.BlogUrl;
		TwitchUrl = member.TwitchUrl;
		YouTubeUrl = member.YouTubeUrl;
		TwitterUrl = member.TwitterUrl;
		GithubUrl = member.GitHubUrl;
		LinkedInUrl = member.LinkedInUrl;
		OtherUrl = member.OtherUrl;
		CodinGameUrl = member.CodinGameUrl;
		MastodonUrl = member.MastodonUrl;
	}

}
