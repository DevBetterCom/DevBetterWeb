using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;

namespace DevBetterWeb.Web.Pages
{
    public class QuestionsModel : PageModel
    {
        private readonly CleanArchitecture.Infrastructure.Data.AppDbContext _context;

        public QuestionsModel(CleanArchitecture.Infrastructure.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Question> Question { get;set; }

        public async Task OnGetAsync()
        {
            Question = await _context.Questions.ToListAsync();
        }
    }
}
