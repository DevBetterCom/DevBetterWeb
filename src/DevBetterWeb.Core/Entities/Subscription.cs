using DevBetterWeb.Core.SharedKernel;
using System;

namespace DevBetterWeb.Core.Entities
{
    public class Subscription : BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int MemberId { get; set; }
    }
}
