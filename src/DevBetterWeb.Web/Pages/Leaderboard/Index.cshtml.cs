using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Leaderboard
{
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
  public class IndexModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository _repository;

    public List<MemberLinksDTO> Members { get; set; } = new List<MemberLinksDTO>();
    public List<MemberLinksDTO> Alumni { get; set; } = new List<MemberLinksDTO>();
    public List<Book> Books { get; set; } = new List<Book>();

    public IndexModel(UserManager<ApplicationUser> userManager,
        IRepository repository)
    {
      _userManager = userManager;
      _repository = repository;
    }

    public async Task OnGet()
    {
      var usersInMemberRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);
      var usersInAlumniRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ALUMNI);

      var memberUserIds = usersInMemberRole.Select(x => x.Id).ToList();
      var alumniUserIds = usersInAlumniRole.Select(x => x.Id).ToList();

      var memberSpec = new MembersHavingUserIdsWithBooksSpec(memberUserIds);
      var members = await _repository.ListAsync(memberSpec);

      var alumniSpec = new MembersHavingUserIdsWithBooksSpec(alumniUserIds);
      var alumni = await _repository.ListAsync(alumniSpec);

      Members = members
          .Where(m => (m.BooksRead?.Count ?? 0) > 0 && 
                !alumni.Any(alumni => alumni.Id == m.Id))
          .Select(member => MemberLinksDTO.FromMemberEntity(member))
          .ToList();

      Alumni = alumni.Select(alumni => MemberLinksDTO.FromMemberEntity(alumni))
          .Where(m => (m.BooksRead?.Count ?? 0) > 0)
          .ToList();

      var bookSpec = new BooksByMemberReadCountWithMembersWhoHaveReadSpec();
      Books = await _repository.ListAsync(bookSpec);
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
