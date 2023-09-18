using System;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Enums;
using DevBetterWeb.Core.Interfaces;

namespace DevBetterWeb.Core.Services;

public class MemberAddBillingActivityService : IMemberAddBillingActivityService
{
  private readonly IRepository<Member> _memberRepository;
  private readonly IMemberLookupService _memberLookupService;
  
  public MemberAddBillingActivityService(IRepository<Member> memberRepository,
    IMemberLookupService memberLookup)
  {
    _memberRepository = memberRepository;
    _memberLookupService = memberLookup;
  }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  public MemberAddBillingActivityService()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
  {
	  
  }

  /// <summary>
  /// This should only be called after a member has registered and been created.
  /// </summary>
  /// <param name="email"></param>
  /// <param name="amount"></param>
  /// <param name="subscriptionPlanName"></param>
  /// <param name="billingPeriod"></param>
  /// <returns></returns>
  public async Task AddMemberSubscriptionCreationBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod)
  {
    var member = await _memberLookupService.GetMemberByEmailAsync(email);

    member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Subscribed, billingPeriod, amount);
    await _memberRepository.SaveChangesAsync();
  }

  public async Task AddMemberSubscriptionRenewalBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod)
  {
    if (string.IsNullOrEmpty(subscriptionPlanName)) throw new ArgumentNullException(nameof(subscriptionPlanName));

    var member = await _memberLookupService.GetMemberByEmailAsync(email);

    member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Renewed, billingPeriod, amount);
    await _memberRepository.SaveChangesAsync();
  }

  public async Task AddMemberSubscriptionCancellationBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod)
  {
    var member = await _memberLookupService.GetMemberByEmailAsync(email);

    member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Cancelled, billingPeriod);
    await _memberRepository.SaveChangesAsync();
  }

  public async Task AddMemberSubscriptionEndingBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod)
  {
    var member = await _memberLookupService.GetMemberByEmailAsync(email);

    member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Ended, billingPeriod);
    await _memberRepository.SaveChangesAsync();
  }
}
