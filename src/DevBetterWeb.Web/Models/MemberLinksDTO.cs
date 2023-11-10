using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Models;

public class MemberLinksDTO
{
	public bool IsMember { get; set; }
	public string? UserId { get; set; }
	public string? FullName { get; set; }
	public string? BlogUrl { get; private set; }
	public string? GitHubUrl { get; private set; }
	public string? LinkedInUrl { get; private set; }
	public string? OtherUrl { get; private set; }
	public string? TwitchUrl { get; private set; }
	public string? YouTubeUrl { get; private set; }
	public string? TwitterUrl { get; private set; }
	public string? BlueskyUrl { get; private set; }
	public string? PEUsername { get; private set; }
	public string? PEBadgeURL { get; private set; }
	public string? Address { get; private set; }
	public string? CodinGameUrl { get; private set; }
	public int SubscribedDays { get; private set; }

	public static MemberLinksDTO FromMemberEntity(Member member)
	{

		var dto = new MemberLinksDTO
		{
			FullName = member.UserFullName(),
			BlogUrl = member.BlogUrl,
			GitHubUrl = member.GitHubUrl,
			LinkedInUrl = member.LinkedInUrl,
			OtherUrl = member.OtherUrl,
			TwitchUrl = member.TwitchUrl,
			YouTubeUrl = member.YouTubeUrl,
			TwitterUrl = member.TwitterUrl,
			BlueskyUrl = member.BlueskyUrl,
			UserId = member.UserId,
			PEUsername = member.PEUsername,
			PEBadgeURL = $"https://projecteuler.net/profile/{member.PEUsername}.png",
			Address = member.Address,
			CodinGameUrl = member.CodinGameUrl,
			SubscribedDays = member.TotalSubscribedDays()
		};

		if (!(string.IsNullOrEmpty(dto.YouTubeUrl)) && !(dto.YouTubeUrl.Contains("?")))
		{
			dto.YouTubeUrl = dto.YouTubeUrl + "?sub_confirmation=1";
		}

		return dto;
	}
}
