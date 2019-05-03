using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [Authorize(Roles = Constants.Roles.ADMINISTRATORS)]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ArchiveVideoEditDTO ArchiveVideoModel { get; set; }

        public class ArchiveVideoEditDTO
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
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archiveVideoEntity = await _context.ArchiveVideos.FirstOrDefaultAsync(m => m.Id == id);

            if (archiveVideoEntity == null)
            {
                return NotFound();
            }
            ArchiveVideoModel = new ArchiveVideoEditDTO
            {
                Id = archiveVideoEntity.Id,
                DateCreated = archiveVideoEntity.DateCreated,
                ShowNotes = archiveVideoEntity.ShowNotes,
                Title = archiveVideoEntity.Title,
                VideoUrl = archiveVideoEntity.VideoUrl
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var currentVideoEntity = await _context.ArchiveVideos.FindAsync(ArchiveVideoModel.Id);

            currentVideoEntity.ShowNotes = ArchiveVideoModel.ShowNotes;
            currentVideoEntity.Title = ArchiveVideoModel.Title;
            currentVideoEntity.VideoUrl = ArchiveVideoModel.VideoUrl;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArchiveVideoExists(ArchiveVideoModel.Id))
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

        private bool ArchiveVideoExists(int id)
        {
            return _context.ArchiveVideos.Any(e => e.Id == id);
        }
    }
}
