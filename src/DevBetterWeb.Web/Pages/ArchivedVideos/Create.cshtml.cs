using CleanArchitecture.Core.Entities;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [Authorize(Roles = Constants.Roles.ADMINISTRATORS)]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ArchiveVideoCreateDTO ArchiveVideoModel { get; set; }

        public class ArchiveVideoCreateDTO
        {
            [Required]
            public string Title { get; set; }
            [DisplayName("Show Notes")]
            public string ShowNotes { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.DateCreated)]
            public DateTimeOffset DateCreated { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.VideoUrl)]
            public string VideoUrl { get; set; }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var videoEntity = new ArchiveVideo()
            {
                DateCreated = ArchiveVideoModel.DateCreated,
                ShowNotes = ArchiveVideoModel.ShowNotes,
                Title = ArchiveVideoModel.Title,
                VideoUrl = ArchiveVideoModel.VideoUrl
            };
            
            _context.ArchiveVideos.Add(videoEntity);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
