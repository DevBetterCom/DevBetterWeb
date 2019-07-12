using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;

namespace DevBetterWeb.Web.Pages
{
    public class QuestionsModel : PageModel
    {
        private readonly DevBetterWeb.Infrastructure.Data.AppDbContext _context;

        public QuestionsModel(DevBetterWeb.Infrastructure.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Question> Question { get; set; }
        public IList<ArchiveVideo> Videos { get; set; }

        public async Task OnGetAsync()
        {
            Question = await _context.Questions.ToListAsync();
            Videos = await _context.ArchiveVideos
                .Include(v => v.Questions)
                .OrderByDescending(v => v.DateCreated)
                .ToListAsync();
        }
    }
}
