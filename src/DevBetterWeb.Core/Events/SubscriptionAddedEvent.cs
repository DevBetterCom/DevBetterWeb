using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class SubscriptionAddedEvent : BaseDomainEvent
{
  public MemberSubscription MemberSubscription { get; }
  
  public SubscriptionAddedEvent(MemberSubscription memberSubscription)
  {
  	MemberSubscription = memberSubscription;
  }
}
