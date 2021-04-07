using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;

namespace DevBetterWeb.Core.Services
{
  public class MemberAddBillingActivityService : IMemberAddBillingActivityService
  {
    private readonly IUserLookupService _userLookup;
    private readonly IMemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;
    private readonly IRepository _repository;

    public MemberAddBillingActivityService(IUserLookupService userLookup,
      IMemberSubscriptionPeriodCalculationsService memberSubscriptionPeriodCalculationsService,
      IRepository repository)
    {
      _userLookup = userLookup;
      _memberSubscriptionPeriodCalculationsService = memberSubscriptionPeriodCalculationsService;
      _repository = repository;
    }

    public async Task AddSubscriptionCreationBillingActivity(string email, decimal amount)
    {
      var member = await GetMemberByEmailAsync(email);

      var message = $"{member.UserFullName()} subscribed to DevBetter";

      member.AddBillingActivity(message, amount);
    }

    public async Task AddSubscriptionRenewalBillingActivity(string email, decimal amount)
    {
      var member = await GetMemberByEmailAsync(email);

      var message = $"{member.UserFullName()} renewed their subscription";

      member.AddBillingActivity(message, amount);
    }

    public async Task AddSubscriptionCancellationBillingActivity(string email)
    {
      var member = await GetMemberByEmailAsync(email);

      var endDate = _memberSubscriptionPeriodCalculationsService.GetCurrentSubscriptionEndDate(member);
      var endDateString = endDate.ToLongDateString();

      var message = $"{member.UserFullName()} cancelled their subscription. They will lose access to DevBetter at the end of their billing cycle, on {endDateString}.";

      member.AddBillingActivity(message);
    }

    public async Task AddSubscriptionEndingBillingActivity(string email)
    {
      var member = await GetMemberByEmailAsync(email);

      var message = $"{member.UserFullName()}'s subscription ended";

      member.AddBillingActivity(message);
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
