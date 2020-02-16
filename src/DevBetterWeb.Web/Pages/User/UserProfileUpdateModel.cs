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
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Address { get; set; }
        [ValidUrl]
        public string LinkedInUrl { get; set; }
        [ValidUrl]
        public string TwitterUrl { get; set; }
        [ValidUrl]
        public string GithubUrl { get; set; }
        [ValidUrl]
        public string BlogUrl { get; set; }
        [ValidUrl]
        public string TwitchUrl { get; set; }
        [ValidUrl]
        public string OtherUrl { get; set; }
        public string AboutInfo { get; set; }

        public UserProfileUpdateModel()
        {

        }

        public UserProfileUpdateModel(ApplicationUser applicationUser)
        {
            BlogUrl = applicationUser.BlogUrl;
            TwitchUrl = applicationUser.TwitchUrl;
            TwitterUrl = applicationUser.TwitterUrl;
            GithubUrl = applicationUser.GithubUrl;
            LinkedInUrl = applicationUser.LinkedInUrl;
            OtherUrl = applicationUser.OtherUrl;
            AboutInfo = applicationUser.AboutInfo;
            FirstName = applicationUser.FirstName;
            LastName = applicationUser.LastName;
            Address = applicationUser.Address;
        }
    }
}
