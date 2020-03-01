using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
    public class DeleteModel : PageModel
    {
        private readonly IRepository _repository;

        public DeleteModel(IRepository repository)
        {
            _repository = repository;
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

            var archiveVideo = await _repository.GetByIdAsync<ArchiveVideo>(id.Value);

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

            var archiveVideo = await _repository.GetByIdAsync<ArchiveVideo>(id.Value);

            if (archiveVideo != null)
            {
                _repository.Delete(archiveVideo);
            }

            return RedirectToPage("./Index");
        }
    }
}
