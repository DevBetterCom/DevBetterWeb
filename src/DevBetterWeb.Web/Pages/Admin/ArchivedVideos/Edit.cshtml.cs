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
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.Admin.ArchivedVideos;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
public class EditModel : PageModel
{
  private readonly IRepository<ArchiveVideo> _videoRepository;
  private readonly AppDbContext _context;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IRepository<Member> _memberRepository;

  public EditModel(
	  IRepository<ArchiveVideo> videoRepository, 
	  AppDbContext context,
		UserManager<ApplicationUser> userManager,
	  IRepository<Member> memberRepository)
  {
    _videoRepository = videoRepository;
    _context = context;
    _userManager = userManager;
    _memberRepository = memberRepository;
  }

#nullable disable
  [BindProperty]
  public ArchiveVideoEditDTO ArchiveVideoModel { get; set; }
#nullable enable
  public List<Question> Questions { get; set; } = new List<Question>();


  public class ArchiveVideoEditDTO
  {
    public int Id { get; set; }
    [Required]
    public string? Title { get; set; }

    [DisplayName(DisplayConstants.ArchivedVideo.DateCreated)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTimeOffset DateCreated { get; set; }

    [DisplayName(DisplayConstants.ArchivedVideo.VideoUrl)]
    public string? VideoUrl { get; set; }
  }

  public async Task<IActionResult> OnGetAsync(int? id)
  {
    if (id == null)
    {
      return NotFound();
    }

    // TODO: use repo + spec here
    var archiveVideoEntity = await _context.ArchiveVideos!
        .AsNoTracking()
        //.Include(v => v.Questions)
        .FirstOrDefaultAsync(v => v.Id == id);


    if (archiveVideoEntity == null)
    {
      return NotFound();
    }
    ArchiveVideoModel = new ArchiveVideoEditDTO
    {
      Id = archiveVideoEntity.Id,
      DateCreated = archiveVideoEntity.DateCreated,
      Title = archiveVideoEntity.Title,
      VideoUrl = archiveVideoEntity.VideoUrl
    };

    //Questions = archiveVideoEntity.Questions;

    return Page();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!ModelState.IsValid)
    {
      return Page();
    }

    var currentVideoEntity = await _videoRepository.GetByIdAsync(ArchiveVideoModel.Id);
    if (currentVideoEntity == null)
    {
      return NotFound();
    }

    currentVideoEntity.Title = ArchiveVideoModel.Title;
    currentVideoEntity.VideoUrl = ArchiveVideoModel.VideoUrl;

    await _videoRepository.UpdateAsync(currentVideoEntity);

    return RedirectToPage("./Index");
  }

  public IActionResult OnPostEditQuestion(int questionId, string questionText, int timestamp)
  {
    var question = _context.Questions!.FirstOrDefault(x => x.Id == questionId);

    if (question == null)
    {
      return BadRequest();
    }
    question.UpdateQuestion(questionText);

    _context.SaveChanges();


    return RedirectToPage("edit", new { id = question.ArchiveVideoId });
  }

  public async Task<IActionResult> OnPostAddQuestion(int archiveVideoId, string questionText, int timestamp)
  {
		var currentUserName = User.Identity!.Name;
		var applicationUser = await _userManager.FindByNameAsync(currentUserName);

		var memberSpec = new MemberByUserIdWithFavoriteArchiveVideosSpec(applicationUser.Id);
		var member = await _memberRepository.FirstOrDefaultAsync(memberSpec);
		if (member is null)
		{
			return Unauthorized();
		}

		var question = new Question(member.Id, questionText);
    question.SetArchiveVideoId(archiveVideoId);

    _context.Questions!.Add(question);

    await _context.SaveChangesAsync();

    return RedirectToPage("edit", new { id = archiveVideoId });
  }

  public IActionResult OnPostDeleteQuestion(int questionId)
  {
    var question = _context.Questions!.FirstOrDefault(x => x.Id == questionId);

    if (question == null)
    {
      return BadRequest();
    }

    var archiveVideoId = question.ArchiveVideoId;
    _context.Questions!.Remove(question);
    _context.SaveChanges();


    return RedirectToPage("edit", new { id = question.ArchiveVideoId });
  }
}
