using DevBetterWeb.Core.SharedKernel;
using System;

namespace DevBetterWeb.Core.Entities
{
    public class Member : BaseEntity
    {
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; } // TODO: Use an actual type?
        public string? LinkedInUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? GithubUrl { get; set; }
        public string? BlogUrl { get; set; }
        public string? TwitchUrl { get; set; }
        public string? OtherUrl { get; set; }
        public string? AboutInfo { get; set; }
        public DateTime DateCreated { get; private set; } = DateTime.UtcNow;
        public string UserFullName()
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
            {
                return "[No Name Provided]";
            }
            else
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
