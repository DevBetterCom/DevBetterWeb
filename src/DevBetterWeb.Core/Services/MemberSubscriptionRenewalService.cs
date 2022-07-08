using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;

namespace DevBetterWeb.Core.Services;

public class MemberSubscriptionRenewalService : IMemberSubscriptionRenewalService
{
  private readonly IUserLookupService _userLookup;
  private readonly IRepository<Member> _memberRepository;

  public MemberSubscriptionRenewalService(IUserLookupService userLookup,
    IRepository<Member> memberRepository)
  {
    _userLookup = userLookup;
    _memberRepository = memberRepository;
  }

  public async Task ExtendMemberSubscription(string email, System.DateTime subscriptionEndDate)
  {
    var userId = await _userLookup.FindUserIdByEmailAsync(email);

    var spec = new MemberByUserIdSpec(userId);
    var member = await _memberRepository.FirstOrDefaultAsync(spec);
    if (member is null) throw new MemberWithEmailNotFoundException(email);
    member.ExtendCurrentSubscription(subscriptionEndDate);
  }
}
