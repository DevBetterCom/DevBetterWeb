using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.Admin.SubscriptionPlans;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class DeleteModel : PageModel
{
  private readonly DevBetterWeb.Infrastructure.Data.AppDbContext _context;

  public DeleteModel(DevBetterWeb.Infrastructure.Data.AppDbContext context)
  {
    _context = context;
  }

  [BindProperty]
  public MemberSubscriptionPlan? MemberSubscriptionPlan { get; set; }

  public async Task<IActionResult> OnGetAsync(int? id)
  {
    if (id == null)
    {
      return NotFound();
    }

    MemberSubscriptionPlan = await _context.MemberSubscriptionPlan!.AsQueryable().FirstOrDefaultAsync(m => m.Id == id);

    if (MemberSubscriptionPlan == null)
    {
      return NotFound();
    }
    return Page();
  }

  public async Task<IActionResult> OnPostAsync(int? id)
  {
    if (id == null)
    {
      return NotFound();
    }

    MemberSubscriptionPlan = await _context.MemberSubscriptionPlan!.FindAsync(id);

    if (MemberSubscriptionPlan != null)
    {
      _context.MemberSubscriptionPlan.Remove(MemberSubscriptionPlan);
      await _context.SaveChangesAsync();
    }

    return RedirectToPage("./Index");
  }
}
