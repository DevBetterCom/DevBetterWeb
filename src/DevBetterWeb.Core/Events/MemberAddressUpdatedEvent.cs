using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events
{
  public class MemberAddressUpdatedEvent : BaseDomainEvent
  {
    public MemberAddressUpdatedEvent(Member member)
    {
      Member = member;
    }

    public Member Member { get; }
  }
}
