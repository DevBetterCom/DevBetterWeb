using DevBetterWeb.Core.Entities;
using DevBetterWeb.Web.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.User
{
    public class UserProfileUpdateModel
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public string? Address { get; set; }
        [ValidLinkedInUrl]
        public string? LinkedInUrl { get; set; }
        [ValidTwitterUrl]
        public string? TwitterUrl { get; set; }
        [ValidGitHubUrl]
        public string? GithubUrl { get; set; }
        [ValidUrl]
        public string? BlogUrl { get; set; }
        [ValidTwitchUrl]
        public string? TwitchUrl { get; set; }
        [ValidYouTubeUrl]
        public string? YouTubeUrl { get; set; }
        [ValidUrl]
        public string? OtherUrl { get; set; }
    [ValidCodinGameUrl]
    public string? CodinGameUrl { get; set; }
        public string? AboutInfo { get; set; }
        public string? PEFriendCode { get; set; }
        public string? PEUsername { get; set; }

        public UserProfileUpdateModel()
        {

        }

        public UserProfileUpdateModel(Member member)
        {
            BlogUrl = member.BlogUrl;
            TwitchUrl = member.TwitchUrl;
            YouTubeUrl = member.YouTubeUrl;
            TwitterUrl = member.TwitterUrl;
            GithubUrl = member.GitHubUrl;
            LinkedInUrl = member.LinkedInUrl;
            OtherUrl = member.OtherUrl;
            AboutInfo = member.AboutInfo;
            FirstName = member.FirstName;
            LastName = member.LastName;
            Address = member.Address;
            PEFriendCode = member.PEFriendCode;
            PEUsername = member.PEUsername;
      CodinGameUrl = member.CodinGameUrl;
        }
    }
}
