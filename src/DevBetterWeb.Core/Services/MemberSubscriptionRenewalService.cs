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
	private readonly IAppLogger<MemberSubscriptionRenewalService> _logger;

	public MemberSubscriptionRenewalService(IUserLookupService userLookup,
    IRepository<Member> memberRepository,
		IAppLogger<MemberSubscriptionRenewalService> logger)
  {
    _userLookup = userLookup;
    _memberRepository = memberRepository;
		_logger = logger;
	}

  public async Task ExtendMemberSubscription(string email, System.DateTime subscriptionEndDate)
  {
    var userId = await _userLookup.FindUserIdByEmailAsync(email);

    var spec = new MemberByUserIdSpec(userId);
    var member = await _memberRepository.FirstOrDefaultAsync(spec);
    if (member is null) throw new MemberWithEmailNotFoundException(email);
    member.ExtendCurrentSubscription(subscriptionEndDate);

		await _memberRepository.SaveChangesAsync();

		_logger.LogInformation($"Extended {member.UserFullName} subscription to {subscriptionEndDate.ToShortDateString}");
  }
}
