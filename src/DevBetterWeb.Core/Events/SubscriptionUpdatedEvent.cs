using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class SubscriptionUpdatedEvent : BaseDomainEvent
{
  public MemberSubscription MemberSubscription { get; }
  
  public SubscriptionUpdatedEvent(MemberSubscription memberSubscription)
  {
  	MemberSubscription = memberSubscription;
  }
}
