using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Pages.User
{
    public class UserDetailsViewModel
    {
        public string? Name { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? GithubUrl { get; set; }
        public string? BlogUrl { get; set; }
        public string? TwitchUrl { get; set; }
        public string? OtherUrl { get; set; }
        public string? AboutInfo { get; set; }
        public string? PEFriendCode { get; set; }

        public UserDetailsViewModel()
        {
        }

        public UserDetailsViewModel(Member member)
        {
            BlogUrl = member.BlogUrl;
            TwitchUrl = member.TwitchUrl;
            TwitterUrl = member.TwitterUrl;
            GithubUrl = member.GitHubUrl;
            LinkedInUrl = member.LinkedInUrl;
            OtherUrl = member.OtherUrl;
            AboutInfo = member.AboutInfo;
            Name = member.UserFullName();
            PEFriendCode = member.PEFriendCode;
        }
    }
}
