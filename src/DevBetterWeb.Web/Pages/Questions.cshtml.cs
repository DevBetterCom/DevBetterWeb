using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages;

public class QuestionsModel : PageModel
{
  private readonly AppDbContext _context;

  public QuestionsModel(AppDbContext context)
  {
    _context = context;
  }

  public IList<Question> Question { get; set; } = new List<Question>();
  public IList<ArchiveVideo> Videos { get; set; } = new List<ArchiveVideo>();

  public async Task OnGetAsync()
  {
    Question = await _context.Questions!.AsQueryable().ToListAsync();
    Videos = await _context.ArchiveVideos!
        //.Include(v => v.Questions)
        .OrderByDescending(v => v.DateCreated)
        .ToListAsync();
  }
}
