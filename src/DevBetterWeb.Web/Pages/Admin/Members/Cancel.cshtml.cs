using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Pages.Admin.Members
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class CancelModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPaymentHandlerSubscription _paymentHandlerSubscription;
    private readonly IRepository<Member> _memberRepository;

    public string EmailToUnsubscribe { get; private set; }

    public string NameToUnsubscribe { get; private set; }

    public string Message { get; private set; }

    public CancelModel(UserManager<ApplicationUser> userManager,
      IPaymentHandlerSubscription paymentHandlerSubscription,
      IRepository<Member> repository)
    {
      _userManager = userManager;
      _paymentHandlerSubscription = paymentHandlerSubscription;
      _memberRepository = repository;
    }

    public async Task OnGetAsync(string userId)
    {
      var spec = new MemberByUserIdSpec(userId);
      var member = await _memberRepository.GetBySpecAsync(spec);

      var user = await _userManager.FindByIdAsync(userId);
      EmailToUnsubscribe = await _userManager.GetEmailAsync(user);

      if(member != null)
      {
        NameToUnsubscribe = member.UserFullName();
        EmailToUnsubscribe = "";
        Message = $"If you are sure you want to unsubscribe {NameToUnsubscribe} with email {EmailToUnsubscribe} from DevBetter, click below.";
      }
      else
      {
        Message = "Invalid Link. Please try again.";
      }
    }

    public async Task<PageResult> OnPost()
    {
      try
      {
        await _paymentHandlerSubscription.CancelSubscriptionAtPeriodEnd(EmailToUnsubscribe);
        Message = $"{NameToUnsubscribe} has been unsubscribed from DevBetter. They will retain access until the end of their subscription period.";
        return Page();
      }
      catch
      {
        Message = "Attempt to cancel subscription failed.";
        return Page();
      }
    }
  }
}
