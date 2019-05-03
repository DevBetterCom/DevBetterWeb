using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [Authorize(Roles = Constants.Roles.ADMINISTRATORS)]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        protected class DeleteVideoModel
        {
            public int Id { get; set; }
        }

        public ArchiveVideoDeleteDTO ArchiveVideoToDelete { get; set; }

        public class ArchiveVideoDeleteDTO
        {
            public int Id { get; set; }
            public string Title { get; set; }

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

            var archiveVideo = await _context.ArchiveVideos.FirstOrDefaultAsync(m => m.Id == id);

            if (archiveVideo == null)
            {
                return NotFound();
            }
            ArchiveVideoToDelete = new ArchiveVideoDeleteDTO
            {
                Id = archiveVideo.Id,
                Title = archiveVideo.Title,
                DateCreated = archiveVideo.DateCreated,
                VideoUrl = archiveVideo.VideoUrl
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archiveVideo = await _context.ArchiveVideos.FindAsync(id);

            if (archiveVideo != null)
            {
                _context.ArchiveVideos.Remove(archiveVideo);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
