using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DevBetterWeb.Core.Entities
{
    public class Member : BaseEntity
    {
        public Member()
        {
            UserId = "";

            //BooksRead = new JoinCollectionFacade<Book, BookMember>(
            //    BookMembers,
            //    bm => bm.Book,
            //    b => new BookMember { Member = this, Book = b });
        }

        /// <summary>
        /// Members should only be created via the IMemberRegistrationService.
        /// This will fire off a NewMemberCreatedEvent
        /// </summary>
        /// <param name="userId"></param>
        internal Member(string userId)
        {
            UserId = userId;
            Events.Add(new NewMemberCreatedEvent(this));

            //BooksRead = new JoinCollectionFacade<Book, BookMember>(
            //    BookMembers,
            //    bm => bm.Book,
            //    b => new BookMember { Member = this, Book = b });
        }

        public string UserId { get; private set; }
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public string? AboutInfo { get; private set; }
        public string? Address { get; private set; }
        public string? PEFriendCode { get; private set; }
        public string? PEUsername { get; private set; }

        public string? BlogUrl { get; private set; }
        public string? GitHubUrl { get; private set; }
        public string? LinkedInUrl { get; private set; }
        public string? OtherUrl { get; private set; }
        public string? TwitchUrl { get; private set; }
        public string? YouTubeUrl { get; private set; }
        public string? TwitterUrl { get; private set; }

        public ICollection<BookMember>? BookMembers { get; }// = new List<BookMember>();

        //[NotMapped]
        //public ICollection<Book> BooksRead { get; set; }

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

        public void UpdateName(string? firstName, string? lastName)
        {
            bool valueChanged = false;
            if (FirstName != firstName)
            {
                FirstName = firstName;
                valueChanged = true;
            }
            if (LastName != lastName)
            {
                LastName = lastName;
                valueChanged = true;
            }
            if (valueChanged)
            {
                CreateOrUpdateUpdateEvent("Name");
            }
        }

        public void UpdateAddress(string? address)
        {
            if (Address == address) return;

            Address = address;
            CreateOrUpdateUpdateEvent(nameof(Address));
        }

        public void UpdateAboutInfo(string? aboutInfo)
        {
            if (AboutInfo == aboutInfo) return;

            AboutInfo = aboutInfo;
            CreateOrUpdateUpdateEvent(nameof(AboutInfo));
        }
        public void UpdatePEInfo(string? peFriendCode, string? peUsername)
        {
            bool valueChanged = false;

            if (PEFriendCode != peFriendCode)
            {
                PEFriendCode = peFriendCode;
                valueChanged = true;
            }
            if(PEUsername != peUsername)
            {
                PEUsername = peUsername;
                valueChanged = true;
            }

            if(valueChanged)
            {
                CreateOrUpdateUpdateEvent("ProjectEuler");
            }
        }

        public void UpdateLinks(string? blogUrl,
            string? gitHubUrl,
            string? linkedInUrl,
            string? otherUrl,
            string? twitchUrl,
            string? youtubeUrl,
            string? twitterUrl)
        {
            bool valueChanged = false;
            if (BlogUrl != blogUrl)
            {
                BlogUrl = blogUrl;
                valueChanged = true;
            }
            if (GitHubUrl != gitHubUrl)
            {
                GitHubUrl = gitHubUrl;
                valueChanged = true;
            }
            if (LinkedInUrl != linkedInUrl)
            {
                LinkedInUrl = linkedInUrl;
                valueChanged = true;
            }
            if (OtherUrl != otherUrl)
            {
                OtherUrl = otherUrl;
                valueChanged = true;
            }
            if (TwitchUrl != twitchUrl)
            {
                TwitchUrl = twitchUrl;
                valueChanged = true;
            }
            if (YouTubeUrl != youtubeUrl)
            {
                YouTubeUrl = youtubeUrl;
                valueChanged = true;
            }
            if (TwitterUrl != twitterUrl)
            {
                TwitterUrl = twitterUrl;
                valueChanged = true;
            }
            if (valueChanged)
            {
                CreateOrUpdateUpdateEvent("Links");
            }
        }

        /**
        public void UpdateBooks(List<BookMember> booksRead)
        {
            if (BooksRead != booksRead)
            {
                BooksRead = booksRead;
                CreateOrUpdateUpdateEvent("Books");
            }
        }
        **/

        public void AddBookRead(Book book)
        {

            //if (!BooksRead.Any(br => br.Id == book.Id))
            //{
            //    BooksRead.Add(book);
            //    CreateOrUpdateUpdateEvent("Books");
            //}
        }

        //public void RemoveBookRead(Book book)
        //{
        //    var bookToRemove = BooksRead.FirstOrDefault(br => br.Id == book.Id);

        //    if (bookToRemove != null)
        //    {
        //        BookMembers.Remove(bookToRemove);
        //        CreateOrUpdateUpdateEvent("Books");
        //    }
        //}

        private void CreateOrUpdateUpdateEvent(string updateDetails)
        {
            MemberUpdatedEvent? updatedEvent = Events.Find(evt => evt is MemberUpdatedEvent) as MemberUpdatedEvent;

            if (updatedEvent != null)
            {
                updatedEvent.UpdateDetails += "," + updateDetails;
                return;
            }

            updatedEvent = new MemberUpdatedEvent(this, updateDetails);
            Events.Add(updatedEvent);
        }
    }
}
