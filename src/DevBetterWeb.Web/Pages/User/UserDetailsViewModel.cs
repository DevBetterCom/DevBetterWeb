using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Web.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.User
{
    public class UserDetailsViewModel
    {
        public string Name { get; set; }
        public string LinkedInUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string GithubUrl { get; set; }
        public string BlogUrl { get; set; }
        public string TwitchUrl { get; set; }
        public string OtherUrl { get; set; }
        public string AboutInfo { get; set; }

        public UserDetailsViewModel()
        {

        }

        public UserDetailsViewModel(ApplicationUser applicationUser)
        {
            BlogUrl = applicationUser.BlogUrl;
            TwitchUrl = applicationUser.TwitchUrl;
            TwitterUrl = applicationUser.TwitterUrl;
            GithubUrl = applicationUser.GithubUrl;
            LinkedInUrl = applicationUser.LinkedInUrl;
            OtherUrl = applicationUser.OtherUrl;
            AboutInfo = applicationUser.AboutInfo;
            Name = applicationUser.UserFullName();
        }
    }
}
