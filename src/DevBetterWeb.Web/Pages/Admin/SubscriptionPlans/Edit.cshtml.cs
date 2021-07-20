using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Core;
using Microsoft.AspNetCore.Authorization;

namespace DevBetterWeb.Web.Pages.Admin.SubscriptionPlans
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]

    public class EditModel : PageModel
    {
      private readonly AppDbContext _context;

      public EditModel(AppDbContext context)
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

      // To protect from overposting attacks, enable the specific properties you want to bind to.
      // For more details, see https://aka.ms/RazorPagesCRUD.
      public async Task<IActionResult> OnPostAsync()
      {
        if (!ModelState.IsValid)
        {
          return Page();
        }

        _context.Attach(MemberSubscriptionPlan).State = EntityState.Modified;

        try
        {
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!MemberSubscriptionPlanExists(MemberSubscriptionPlan!.Id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }

        return RedirectToPage("./Index");
      }

      private bool MemberSubscriptionPlanExists(int id)
      {
        return _context.MemberSubscriptionPlan!.Any(e => e.Id == id);
      }
    }
  }
