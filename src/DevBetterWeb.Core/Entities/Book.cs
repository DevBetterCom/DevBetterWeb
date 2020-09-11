using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevBetterWeb.Core.Entities
{
    public class Book : BaseEntity
    {
        public List<Member> MembersHaveRead { get; private set; } = new List<Member>();
        
        public string? Title { get; private set; }
        public string? Author { get; private set; }
        public string? Details { get; private set; }
        public string? PurchaseUrl { get; private set; }

    }
}
