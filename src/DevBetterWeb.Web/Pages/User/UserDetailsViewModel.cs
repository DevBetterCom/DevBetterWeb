using System.Collections.Generic;
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
        public string? YouTubeUrl { get; set; }
        public string? OtherUrl { get; set; }
        public string? AboutInfo { get; set; }
        public string? Address { get; set; }
        public string? PEFriendCode { get; set; }
        public string? PEBadgeURL { get; set; }
    public List<Book> BooksRead { get; set; } = new List<Book>();
    public string? CodinGameUrl { get; set; }

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
      CodinGameUrl = member.CodinGameUrl;

            YouTubeUrl = member.YouTubeUrl;
            if(!(string.IsNullOrEmpty(YouTubeUrl)) && !(YouTubeUrl.Contains("?")))
            {
                YouTubeUrl = YouTubeUrl + "?sub_confirmation=1";
            }

            OtherUrl = member.OtherUrl;
            AboutInfo = member.AboutInfo;
            Address = member.Address;
            Name = member.UserFullName();
            PEFriendCode = member.PEFriendCode;
            if (!(string.IsNullOrEmpty(member.PEUsername)))
            {
                PEBadgeURL = $"https://projecteuler.net/profile/{member.PEUsername}.png";
            }

      BooksRead = member.BooksRead!;
        }
    }
}
