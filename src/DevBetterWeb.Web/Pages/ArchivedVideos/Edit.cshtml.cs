using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
    public class EditModel : PageModel
    {
        private readonly IRepository _repository;

        public EditModel(IRepository repository)
        {
            _repository = repository;
        }

        [BindProperty]
        public ArchiveVideoEditDTO ArchiveVideoModel { get; set; }

        public class ArchiveVideoEditDTO
        {
            public int Id { get; set; }
            [Required]
            public string Title { get; set; }
            [DisplayName(DisplayConstants.ArchivedVideo.ShowNotes)]
            public string ShowNotes { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.DateCreated)]
            public DateTimeOffset DateCreated { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.VideoUrl)]
            public string VideoUrl { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archiveVideoEntity = _repository.GetById<ArchiveVideo>(id.Value);

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

            var currentVideoEntity = _repository.GetById<ArchiveVideo>(ArchiveVideoModel.Id);
            if(currentVideoEntity == null)
            {
                return NotFound();
            }

            currentVideoEntity.ShowNotes = ArchiveVideoModel.ShowNotes;
            currentVideoEntity.Title = ArchiveVideoModel.Title;
            currentVideoEntity.VideoUrl = ArchiveVideoModel.VideoUrl;

            _repository.Update(currentVideoEntity);

            return RedirectToPage("./Index");
        }
    }
}
