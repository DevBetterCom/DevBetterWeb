using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Enums;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;

namespace DevBetterWeb.Core.Services
{
  public class MemberAddBillingActivityService : IMemberAddBillingActivityService
  {
    private readonly IUserLookupService _userLookup;
    private readonly IRepository _repository;

    public MemberAddBillingActivityService(IUserLookupService userLookup,
      IRepository repository)
    {
      _userLookup = userLookup;
      _repository = repository;
    }

    public async Task AddSubscriptionCreationBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, "Subscribed", billingPeriod, amount);
    }

    public async Task AddSubscriptionRenewalBillingActivity(string email, decimal amount, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, "Renewed", billingPeriod, amount);
    }

    public async Task AddSubscriptionCancellationBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, "Cancelled", billingPeriod);
    }

    public async Task AddSubscriptionEndingBillingActivity(string email, string subscriptionPlanName, BillingPeriod billingPeriod)
    {
      var member = await GetMemberByEmailAsync(email);

      member.AddBillingActivity(subscriptionPlanName, "Ended", billingPeriod);
    }

    private async Task<Member> GetMemberByEmailAsync(string email)
    {
      var userId = await _userLookup.FindUserIdByEmailAsync(email);
      var spec = new MemberByUserIdSpec(userId);
      var member = await _repository.GetAsync(spec);
      return member;
    }
  }
}
