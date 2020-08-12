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
        [ValidUrl]
        public string? LinkedInUrl { get; set; }
        [ValidUrl]
        public string? TwitterUrl { get; set; }
        [ValidUrl]
        public string? GithubUrl { get; set; }
        [ValidUrl]
        public string? BlogUrl { get; set; }
        [ValidUrl]
        public string? TwitchUrl { get; set; }
        [ValidUrl]
        public string? YouTubeUrl { get; set; }
        [ValidUrl]
        public string? OtherUrl { get; set; }
        public string? AboutInfo { get; set; }

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
        }
    }
}
