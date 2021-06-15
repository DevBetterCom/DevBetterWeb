using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Enums;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;

namespace DevBetterWeb.Core.Services
{
  public class MemberAddBillingActivityService : IMemberAddBillingActivityService
  {
    private readonly IUserLookupService _userLookup;
    private readonly IRepository<Member> _memberRepository;
    private readonly IMemberLookupService _memberLookupService;

    public MemberAddBillingActivityService(IUserLookupService userLookup,
      IRepository<Member> memberRepository,
      IMemberLookupService memberLookup)
    {
      _userLookup = userLookup;
      _memberRepository = memberRepository;
      _memberLookupService = memberLookup;
    }

    public async Task AddMemberSubscriptionCreationBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await _memberLookupService.GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Subscribed, billingPeriod, amount);
    }

    public async Task AddMemberSubscriptionRenewalBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await _memberLookupService.GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Renewed, billingPeriod, amount);
    }

    public async Task AddMemberSubscriptionCancellationBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await _memberLookupService.GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Cancelled, billingPeriod);
    }

    public async Task AddMemberSubscriptionEndingBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await _memberLookupService.GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Ended, billingPeriod);
    }
  }
}
