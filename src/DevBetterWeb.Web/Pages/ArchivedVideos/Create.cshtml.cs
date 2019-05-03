using CleanArchitecture.Core.Entities;
using CleanArchitecture.Core.Interfaces;
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
        private readonly IRepository _repository;

        public CreateModel(IRepository repository)
        {
            _repository = repository;
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
            [DisplayName(DisplayConstants.ArchivedVideo.ShowNotes)]
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

            _repository.Add(videoEntity);

            return RedirectToPage("./Index");
        }
    }
}
