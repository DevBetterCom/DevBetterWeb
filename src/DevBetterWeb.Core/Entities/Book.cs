using DevBetterWeb.Core.SharedKernel;
using System.Collections.Generic;

namespace DevBetterWeb.Core.Entities
{
    public class Book : BaseEntity
    {
        public string? Title { get; set; }
        public string? Author { get;  set; }
        public string? Details { get; set; }
        public string? PurchaseUrl { get; set; }

        public List<BookMember> MembersWhoHaveRead { get; private set; } = new List<BookMember>();

        public override string ToString()
        {
            return Title + " by " + Author;
        }
    }
}
