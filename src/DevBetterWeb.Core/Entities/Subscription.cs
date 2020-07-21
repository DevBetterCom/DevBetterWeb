using DevBetterWeb.Core.SharedKernel;
using System;

namespace DevBetterWeb.Core.Entities
{
    class Subscription : BaseEntity
    {
        public int MemberId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
