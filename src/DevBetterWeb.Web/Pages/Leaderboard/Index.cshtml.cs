using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevBetterWeb.Web.Pages.Leaderboard
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
  public class IndexModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _appDbContext;
    private readonly IRepository _repository;

    public List<MemberLinksDTO> Members { get; set; } = new List<MemberLinksDTO>();
    public List<MemberLinksDTO> Alumni { get; set; } = new List<MemberLinksDTO>();
    public List<Book> Books { get; set; } = new List<Book>();

    public IndexModel(UserManager<ApplicationUser> userManager,
        AppDbContext appDbContext,
        IRepository repository)
    {
      _userManager = userManager;
      _appDbContext = appDbContext;
      _repository = repository;
    }

    public async Task OnGet()
    {
      var usersInMemberRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);
      var usersInAlumniRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ALUMNI);

      // TODO: Write a LINQ join for this
      // TODO: See if we can use a specification here
      var memberUserIds = usersInMemberRole.Select(x => x.Id).ToList();
      var alumniUserIds = usersInAlumniRole.Select(x => x.Id).ToList();
#nullable disable
      var members = await _appDbContext.Members.AsNoTracking()
          .Where(member => memberUserIds.Contains(member.UserId) &&
            !alumniUserIds.Any(id => id == member.UserId))
          .OrderByDescending(member => member.BooksRead.Count)
          .ThenBy(member => member.LastName)
          .ThenBy(member => member.FirstName)
          .Include(member => member.BooksRead)
          .ToListAsync();

      var alumniSpec = new MembersHavingUserIdsWithBooksSpec(alumniUserIds);
      var alumni = await _repository.ListAsync(alumniSpec);

      Members = members.Select(member => MemberLinksDTO.FromMemberEntity(member))
          .Where(m => m.BooksRead.Count > 0)
          .ToList();

      Alumni = alumni.Select(alumni => MemberLinksDTO.FromMemberEntity(alumni))
          .Where(m => m.BooksRead.Count > 0)
          .ToList();

      var books = await _appDbContext.Books.AsQueryable()
        .OrderByDescending(book => book.MembersWhoHaveRead.Count)
        .ThenBy(book => book.Title)
        .Include(book => book.MembersWhoHaveRead)
        .AsNoTracking()
        .ToListAsync();

      Books = books;
#nullable enable

    }

    public class MemberLinksDTO
    {
      public string? UserId { get; set; }
      public string? FullName { get; set; }
      public List<Book>? BooksRead { get; private set; }

      public static MemberLinksDTO FromMemberEntity(Member member)
      {
        var dto = new MemberLinksDTO
        {
          FullName = member.UserFullName(),
          BooksRead = member.BooksRead,
          UserId = member.UserId
        };

        if (dto.BooksRead == null)
        {
          dto.BooksRead = new List<Book>();
        }

        return dto;
      }
    }
  }
}
