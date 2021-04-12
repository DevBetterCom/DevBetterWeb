using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DevBetterWeb.Core;
using DevBetterWeb.Infrastructure.Identity.Data;

namespace DevBetterWeb.Web.Pages.User.MyProfile
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class BillingModel : PageModel
  {
#nullable disable
    public UserBillingViewModel UserBillingViewModel { get; private set; }
#nullable enable
    private readonly IRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMemberRegistrationService _memberRegistrationService;

    public BillingModel(IRepository repository, 
      UserManager<ApplicationUser> userManager,
      IMemberRegistrationService memberRegistrationService)
    {
      _repository = repository;
      _userManager = userManager;
      _memberRegistrationService = memberRegistrationService;
    }

    public async Task OnGetAsync()
    {
      var currentUserName = User.Identity!.Name;
      var applicationUser = await _userManager.FindByNameAsync(currentUserName);

      var spec = new MemberByUserIdWithBillingActivitiesSpec(applicationUser.Id);
      var member = await _repository.GetAsync(spec);

      if (member == null)
      {
        member = await _memberRegistrationService.RegisterMemberAsync(applicationUser.Id);
      }

      UserBillingViewModel = new UserBillingViewModel(member);
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

        message += $"You are currently subscribed to a {UserBillingViewModel.SubscriptionPeriod} plan. Your current subscription expires on {UserBillingViewModel.CurrentSubscriptionEndDate} and will be automatically renewed at that time. ";
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
