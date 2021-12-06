using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.Admin.SubscriptionPlans;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class IndexModel : PageModel
{
  private readonly AppDbContext _context;

  public IndexModel(AppDbContext context)
  {
    _context = context;
  }

  public IList<MemberSubscriptionPlan>? MemberSubscriptionPlansList { get; set; }

  public async Task OnGetAsync()
  {
    MemberSubscriptionPlansList = await _context.MemberSubscriptionPlan!.AsQueryable().ToListAsync();
  }
}
