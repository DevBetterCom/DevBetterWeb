using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DevBetterWeb.Web.Pages.Admin.ArchivedVideos;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS)]
public class DetailsModel : PageModel
{
  private readonly IConfiguration _configuration;
  private readonly IRepository<ArchiveVideo> _videoRepository;

  public DetailsModel(IConfiguration configuration,
      IRepository<ArchiveVideo> videoRepository)
  {
    _configuration = configuration;
    _videoRepository = videoRepository;
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

    var spec = new ArchiveVideoWithQuestionsSpec(id.Value);
    var archiveVideoEntity = await _videoRepository.GetBySpecAsync(spec);

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
