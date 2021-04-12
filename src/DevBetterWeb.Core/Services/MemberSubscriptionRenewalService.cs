using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Services
{
  public class MemberSubscriptionRenewalService : IMemberSubscriptionRenewalService
  {
    private readonly IUserLookupService _userLookup;
    private readonly IRepository _repository;

    public MemberSubscriptionRenewalService(IUserLookupService userLookup,
      IRepository repository)
    {
      _userLookup = userLookup;
      _repository = repository;
    }

    public async Task ExtendMemberSubscription(string email, System.DateTime subscriptionEndDate)
    {
      var userId = await _userLookup.FindUserIdByEmailAsync(email);

      var spec = new MemberByUserIdSpec(userId);
      var member = await _repository.GetAsync(spec);

      member.ExtendCurrentSubscription(subscriptionEndDate);
    }
  }
}
