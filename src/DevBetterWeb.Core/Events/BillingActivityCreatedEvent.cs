using Ardalis.GuardClauses;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Events;

public class BillingActivityCreatedEvent : BaseDomainEvent
{
  public BillingActivity BillingActivity { get; }
  public Member Member { get; }
	
  public BillingActivityCreatedEvent(BillingActivity billingActivity, Member member)
  {
	BillingActivity = Guard.Against.Null(billingActivity, nameof(billingActivity));
	Member = Guard.Against.Null(member, nameof(member));
  }
}
