using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.ArchivedVideos
{
    [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
    public class IndexModel : PageModel
    {
        private readonly IRepository _repository;

        public IndexModel(IRepository repository)
        {
            _repository = repository;
        }

        public IList<ArchiveVideoIndexDTO> ArchiveVideoList { get; set; }

        public class ArchiveVideoIndexDTO
        {
            public int Id { get; set; }
            public string Title { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.DateCreated)]
            public DateTimeOffset DateCreated { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.VideoUrl)]
            public string VideoUrl { get; set; }
        }

        public async Task OnGetAsync()
        {
            ArchiveVideoList = _repository.List<ArchiveVideo>() 
                .Select(v => new ArchiveVideoIndexDTO
                {
                    Id = v.Id,
                    DateCreated = v.DateCreated,
                    Title = v.Title,
                    VideoUrl = v.VideoUrl
                })
                .ToList();
        }
    }
}
