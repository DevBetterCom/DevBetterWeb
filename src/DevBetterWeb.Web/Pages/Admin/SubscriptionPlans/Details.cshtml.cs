using System;
using System.Collections.Generic;
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
  public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

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
    }
}
