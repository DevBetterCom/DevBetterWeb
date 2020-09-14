using DevBetterWeb.Core.SharedKernel;
using System.Collections.Generic;

namespace DevBetterWeb.Core.Entities
{
    public class Book : BaseEntity
    {
        public string? Title { get; private set; }
        public string? Author { get; private set; }
        public string? Details { get; private set; }
        public string? PurchaseUrl { get; private set; }

        public List<BookMember>? MembersWhoHaveRead { get; private set; }

        public override string ToString()
        {
            return Title + "by" + Author;
        }

    }
}
