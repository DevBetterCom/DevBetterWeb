using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DevBetterWeb.Core;
using DevBetterWeb.Infrastructure.Identity.Data;
using DevBetterWeb.Core.Entities;
using System;
using System.Linq;

namespace DevBetterWeb.Web.Pages.User.MyProfile
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class BillingModel : PageModel
  {
#nullable disable
    public UserBillingViewModel UserBillingViewModel { get; private set; }

#nullable enable
    private readonly IRepository<Member> _memberRepository;
    private readonly IRepository<MemberSubscriptionPlan> _subscriptionPlanRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMemberRegistrationService _memberRegistrationService;
    private readonly IMemberSubscriptionPeriodCalculationsService _memberSubscriptionPeriodCalculationsService;

    public BillingModel(IRepository<Member> memberRepository, 
      IRepository<MemberSubscriptionPlan> subscriptionPlanRepository,
      UserManager<ApplicationUser> userManager,
      IMemberRegistrationService memberRegistrationService,
      IMemberSubscriptionPeriodCalculationsService memberSubscriptionPeriodCalculationsService)
    {
      _memberRepository = memberRepository;
      _subscriptionPlanRepository = subscriptionPlanRepository;
      _userManager = userManager;
      _memberRegistrationService = memberRegistrationService;
      _memberSubscriptionPeriodCalculationsService = memberSubscriptionPeriodCalculationsService;
    }

    public async Task OnGetAsync()
    {
      var currentUserName = User.Identity!.Name;
      var applicationUser = await _userManager.FindByNameAsync(currentUserName);

      var spec = new MemberByUserIdWithBillingActivitiesSpec(applicationUser.Id);
      var member = await _memberRepository.GetBySpecAsync(spec);

      if (member == null)
      {
        member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
      }

      var billingActivities = member.BillingActivities.OrderBy(a => a.Details.Date).ToList();


      if (_memberSubscriptionPeriodCalculationsService.GetHasCurrentSubscription(member))
      {
        var currentSubscription = _memberSubscriptionPeriodCalculationsService.GetCurrentSubscription(member);
        if (currentSubscription is null) throw new Exception($"Member {member} has no current subscription.");

        var subscriptionPlan = await _subscriptionPlanRepository.GetByIdAsync(currentSubscription.MemberSubscriptionPlanId);
        var currentSubscriptionEndDate = _memberSubscriptionPeriodCalculationsService.GetCurrentSubscriptionEndDate(member);
        var graduationDate = _memberSubscriptionPeriodCalculationsService.GetGraduationDate(member);


        UserBillingViewModel = new UserBillingViewModel(billingActivities, member.TotalSubscribedDays(), subscriptionPlan!.Details!.Name, subscriptionPlan!.Details!.BillingPeriod, currentSubscriptionEndDate, graduationDate, currentSubscription);
      }
      else
      {
        UserBillingViewModel = new UserBillingViewModel(billingActivities, member.TotalSubscribedDays(), _memberSubscriptionPeriodCalculationsService.GetGraduationDate(member));
      }

    }

    public string GetOverviewBody()
    {
      string message = $"You have been subscribed to DevBetter for {UserBillingViewModel.TotalSubscribedDays} days. ";

      if (UserBillingViewModel.CurrentSubscription != null)
      {
        if (!User.IsInRole(AuthConstants.Roles.ALUMNI))
        {
          message += $"If there is no interruption to your subscription, you will graduate to Alumnus status on {UserBillingViewModel.GraduationDate}.\n";
        }

        message += $"You are currently subscribed to a {UserBillingViewModel.BillingPeriod} plan. Your current subscription expires on {UserBillingViewModel.CurrentSubscriptionEndDate} and will be automatically renewed at that time. ";
      }

      return message;
    }

    public string GetCancellationBody()
    {
      string message = "";

      if (!User.IsInRole(AuthConstants.Roles.ALUMNI))
      {
        message = $"To cancel your DevBetter subscription, click below. Your cancellation will take effect at the end of your current subscription period, on {UserBillingViewModel.CurrentSubscriptionEndDate.ToString()}.";
      }

      return message;
    }

    public bool GetIsAlumni()
    {
      return User.IsInRole(AuthConstants.Roles.ALUMNI);
    }
  }
}
