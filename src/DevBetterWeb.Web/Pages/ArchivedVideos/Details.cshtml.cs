using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;
using DevBetterWeb.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [Authorize(Roles = Constants.Roles.ADMINISTRATORS_MEMBERS)]
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public ArchiveVideoDetailsDTO ArchiveVideoDetails { get; set; }

        public class ArchiveVideoDetailsDTO
        {
            public int Id { get; set; }
            [Required]
            public string Title { get; set; }
            [DisplayName("Show Notes")]
            public string ShowNotes { get; set; }

            [DisplayName("Date Created")]
            public DateTimeOffset DateCreated { get; set; }

            [DisplayName("Video URL")]
            public string VideoUrl { get; set; }

            public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archiveVideoEntity = await _context.ArchiveVideos
                .Include(v => v.Questions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (archiveVideoEntity == null)
            {
                return NotFound();
            }

            ArchiveVideoDetails = new ArchiveVideoDetailsDTO
            {
                DateCreated = archiveVideoEntity.DateCreated,
                ShowNotes = archiveVideoEntity.ShowNotes,
                Title = archiveVideoEntity.Title,
                VideoUrl = archiveVideoEntity.VideoUrl
            };

            ArchiveVideoDetails.Questions.AddRange(
                archiveVideoEntity.Questions
                    .Select(q => new QuestionViewModel
                    {
                        QuestionText = q.QuestionText,
                        TimestampSeconds = q.TimestampSeconds
                    }));

            return Page();
        }
    }
}
