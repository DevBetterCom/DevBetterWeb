using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.DomainEvents;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
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
        public string? PEFriendCode { get; set; }
        public string? PEUsername { get; set; }
        public List<BookMember> BooksRead { get; set; } = new List<BookMember>();
        public int? AddedBook { get; set; }

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
            BooksRead = member.BooksRead;
            
        }

        public bool HasReadBook(Book book)
        {
            if (BooksRead == null)
            {
                return false;
            }

            foreach (BookMember bookMember in BooksRead)
            {
                if (bookMember != null && bookMember.Book != null && bookMember.Book.Equals(book))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
