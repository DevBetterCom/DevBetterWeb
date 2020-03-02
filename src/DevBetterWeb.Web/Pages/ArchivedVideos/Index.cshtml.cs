﻿using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        public IList<ArchiveVideoIndexDTO> ArchiveVideoList { get; set; } = new List<ArchiveVideoIndexDTO>();

        public class ArchiveVideoIndexDTO
        {
            public int Id { get; set; }
            public string? Title { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.DateCreated)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTimeOffset DateCreated { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.VideoUrl)]
            public string? VideoUrl { get; set; }
        }

        public async Task OnGetAsync()
        {
            ArchiveVideoList = (await _repository.ListAsync<ArchiveVideo>()) 
                .Select(video => new ArchiveVideoIndexDTO
                {
                    Id = video.Id,
                    DateCreated = video.DateCreated,
                    Title = video.Title,
                    VideoUrl = video.VideoUrl
                })
                .ToList();
        }
    }
}
