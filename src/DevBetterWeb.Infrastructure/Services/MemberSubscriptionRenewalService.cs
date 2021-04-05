using System.Threading.Tasks;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Core.ValueObjects;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Services
{
  public class MemberSubscriptionRenewalService : IMemberSubscriptionRenewalService
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository _repository;
    private readonly MemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;

    public MemberSubscriptionRenewalService(UserManager<ApplicationUser> userManager,
      IRepository repository,
      MemberSubscriptionPeriodCalculationsService memberSubscriptionPeriodCalculationsService)
    {
      _userManager = userManager;
      _repository = repository;
      _memberSubscriptionPeriodCalculationsService = memberSubscriptionPeriodCalculationsService;
    }

    public async Task ExtendMemberSubscription(string email, System.DateTime subscriptionEndDate)
    {
      var user = await _userManager.FindByEmailAsync(email);

      var spec = new MemberByUserIdSpec(user.Id);
      var member = await _repository.GetAsync(spec);

      var subscription = _memberSubscriptionPeriodCalculationsService.GetCurrentSubscription(member);

      var dates = new DateTimeRange(subscription.Dates.StartDate, subscriptionEndDate);
      subscription.Dates = dates;

      member.UpdateSubscription(subscription);
    }
  }
}
