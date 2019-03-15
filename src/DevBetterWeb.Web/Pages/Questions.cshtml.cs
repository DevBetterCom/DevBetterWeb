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

        public IList<Question> Question { get; set; }
        public IList<ArchiveVideo> Videos { get; set; }

        public async Task OnGetAsync()
        {
            // seed data
            if(_context.ArchiveVideos.Count() == 0)
            {
                var vid1 = new ArchiveVideo()
                {
                    Title = "Video One",
                    DateCreated =new DateTime(2019, 3, 8)
                };
                var vid2 = new ArchiveVideo()
                {
                    Title = "Video Two",
                    DateCreated = new DateTime(2019, 3, 15)
                };
                var questionA = new Question() { QuestionText = "How do I A?" };
                var questionB = new Question() { QuestionText = "How do I B?" };
                vid1.Questions.Add(questionA);
                vid1.Questions.Add(questionB);

                _context.ArchiveVideos.Add(vid1);
                _context.ArchiveVideos.Add(vid2);
                _context.SaveChanges();
            }

            Question = await _context.Questions.ToListAsync();
            Videos = await _context.ArchiveVideos
                .Include(v => v.Questions)
                .OrderByDescending(v => v.DateCreated)
                .ToListAsync();
        }
    }
}
