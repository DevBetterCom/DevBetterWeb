using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Web.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string LinkedInUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string GithubUrl { get; set; }
        public string BlogUrl { get; set; }
        public string TwitchUrl { get; set; }
        public string OtherUrl { get; set; }
        public string AboutInfo { get; set; }

    }
}
