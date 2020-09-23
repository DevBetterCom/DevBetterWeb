using DevBetterWeb.Core.SharedKernel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DevBetterWeb.Core.Entities
{
    public class Book : BaseEntity
    {
        public string? Title { get; set; }
        public string? Author { get;  set; }
        public string? Details { get; set; }
        public string? PurchaseUrl { get; set; }

        public Book()
            => MembersWhoHaveRead = new JoinCollectionFacade<Member, BookMember>(
                BookMembers,
                m => m.Member,
                mb => new BookMember { Member = mb, Book = this });

        public ICollection<BookMember> BookMembers { get; } = new List<BookMember>();
        [NotMapped]
        public ICollection<Member> MembersWhoHaveRead { get; }

        public override string ToString()
        {
            return Title + " by " + Author;
        }
    }
}
