using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.Admin.ArchivedVideos
{
    [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS)]
    public class EditModel : PageModel
    {
        private readonly IRepository _repository;
        private readonly AppDbContext _context;

        public EditModel(IRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
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
            [DisplayName(DisplayConstants.ArchivedVideo.ShowNotes)]
            public string? ShowNotes { get; set; }

            [DisplayName(DisplayConstants.ArchivedVideo.DateCreated)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode =true)]
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

            var archiveVideoEntity = await _context.ArchiveVideos.AsNoTracking()
                .Include(v => v.Questions)
                .FirstOrDefaultAsync(v => v.Id == id);
                

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

            Questions = archiveVideoEntity.Questions;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var currentVideoEntity = await _repository.GetByIdAsync<ArchiveVideo>(ArchiveVideoModel.Id);
            if(currentVideoEntity == null)
            {
                return NotFound();
            }

            currentVideoEntity.ShowNotes = ArchiveVideoModel.ShowNotes;
            currentVideoEntity.Title = ArchiveVideoModel.Title;
            currentVideoEntity.VideoUrl = ArchiveVideoModel.VideoUrl;

            await _repository.UpdateAsync(currentVideoEntity);

            return RedirectToPage("./Index");
        }

        public IActionResult OnPostEditQuestion(int questionId, string questionText, int timestamp)
        {
            var question = _context.Questions!.FirstOrDefault(x => x.Id == questionId);

            if (question == null)
            {
                return BadRequest();
            }

            question.QuestionText = questionText;
            question.TimestampSeconds = timestamp;

            _context.SaveChanges();


            return RedirectToPage("edit", new { id = question.ArchiveVideoId });
        }

        public IActionResult OnPostAddQuestion(int archiveVideoId, string questionText, int timestamp)
        {
            var question = new Question();
            question.ArchiveVideoId = archiveVideoId;
            question.QuestionText = questionText;
            question.TimestampSeconds = timestamp;

            _context.Questions!.Add(question);

            _context.SaveChanges();

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
}
