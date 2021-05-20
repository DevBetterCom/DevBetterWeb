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

    public MemberAddBillingActivityService(IUserLookupService userLookup,
      IRepository<Member> memberRepository)
    {
      _userLookup = userLookup;
      _memberRepository = memberRepository;
    }

    public async Task AddSubscriptionCreationBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Subscribed, billingPeriod, amount);
    }

    public async Task AddSubscriptionRenewalBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Renewed, billingPeriod, amount);
    }

    public async Task AddSubscriptionCancellationBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Cancelled, billingPeriod);
    }

    public async Task AddSubscriptionEndingBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, BillingActivityVerb.Ended, billingPeriod);
    }

    private async Task<Member> GetMemberByEmailAsync(string email)
    {
      var userId = await _userLookup.FindUserIdByEmailAsync(email);
      var spec = new MemberByUserIdSpec(userId);
      var member = await _memberRepository.GetBySpecAsync(spec);
      if (member is null) throw new MemberWithEmailNotFoundException(email);
      return member;
    }
  }
}
