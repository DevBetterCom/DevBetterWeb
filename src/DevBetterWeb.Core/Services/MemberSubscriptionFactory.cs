using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Exceptions;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Services
{
  public class MemberSubscriptionFactory : IMemberSubscriptionFactory
  {
    IRepository<Member> _memberRepository;

    public MemberSubscriptionFactory(IRepository<Member> repository)
    {
      _memberRepository = repository;
    }

    public async Task CreateSubscriptionForMemberAsync(int memberId, DateTimeRange subscriptionDateTimeRange)
    {
      var subscription = new Subscription();
      subscription.MemberId = memberId;
      subscription.Dates = subscriptionDateTimeRange;

      var member = await _memberRepository.GetByIdAsync(memberId);
      if (member is null) throw new MemberNotFoundException(memberId);
      member.AddSubscription(subscription);
    }
  }
}
