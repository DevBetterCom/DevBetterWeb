using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
    public class DetailsModel : PageModel
    {
        // TODO: Refactor to use repository + specification pattern with Include
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public DetailsModel(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public ArchiveVideoDetailsDTO? ArchiveVideoDetails { get; set; }
        public int? StartTime { get; set; }

        public class ArchiveVideoDetailsDTO
        {
            public int Id { get; set; }
            [Required]
            public string? Title { get; set; }
            [DisplayName(DisplayConstants.ArchivedVideo.ShowNotes)]
            public string? ShowNotes { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.DateCreated)]
            public DateTimeOffset DateCreated { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.VideoUrl)]
            public string? VideoUrl { get; set; }

            public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
            
        }

        public async Task<IActionResult> OnGetAsync(int? id, int? startTime = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            StartTime = startTime;

            var archiveVideoEntity = await _context.ArchiveVideos
                .Include(v => v.Questions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (archiveVideoEntity == null)
            {
                return NotFound();
            }

            string videoUrl = _configuration["videoUrlPrefix"] + archiveVideoEntity.VideoUrl;

            ArchiveVideoDetails = new ArchiveVideoDetailsDTO
            {
                DateCreated = archiveVideoEntity.DateCreated,
                ShowNotes = archiveVideoEntity.ShowNotes,
                Title = archiveVideoEntity.Title,
                VideoUrl = videoUrl,
                Id = archiveVideoEntity.Id
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
