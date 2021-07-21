using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using DevBetterWeb.Core;

namespace DevBetterWeb.Web.Pages.Admin.SubscriptionPlans
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
  public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

    [BindProperty]
    public MemberSubscriptionPlan MemberSubscriptionPlan { get; set; } = new();

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.MemberSubscriptionPlan.Add(MemberSubscriptionPlan);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
