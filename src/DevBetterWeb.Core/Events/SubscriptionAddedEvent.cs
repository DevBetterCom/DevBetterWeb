using Ardalis.GuardClauses;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class SubscriptionAddedEvent : BaseDomainEvent
{
  public Member Member { get; }
  public MemberSubscription MemberSubscription { get; }
  
  public SubscriptionAddedEvent(Member member, MemberSubscription memberSubscription)
  {
    Member = Guard.Against.Null(member, nameof(member));
    MemberSubscription = Guard.Against.Null(memberSubscription, nameof(memberSubscription));
  }
}
