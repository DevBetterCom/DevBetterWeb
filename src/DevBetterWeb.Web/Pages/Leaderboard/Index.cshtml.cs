using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevBetterWeb.Web.Pages.Leaderboard;

[Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
public class IndexModel : PageModel
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IRepository<Member> _memberRepository;
  private readonly IRepository<Book> _bookRepository;
  private readonly RankingService<int> _rankingService = new RankingService<int>();

  public List<MemberLinksDTO> Members { get; set; } = new List<MemberLinksDTO>();
  public List<MemberLinksDTO> Alumni { get; set; } = new List<MemberLinksDTO>();
  public List<Book> Books { get; set; } = new List<Book>();

  public Dictionary<int, int> BookRanks { get; set; } = new Dictionary<int, int>();


  public IndexModel(UserManager<ApplicationUser> userManager,
      IRepository<Member> memberRepository,
      IRepository<Book> bookRepository)
  {
    _userManager = userManager;
    _memberRepository = memberRepository;
    _bookRepository = bookRepository;
  }

  public Dictionary<int, int> CalculateMemberBookRanks(List<MemberLinksDTO> members)
  {
    // could probably do this with a groupby bookcount
    var memberBookCounts = members
            .Select(m => m.BooksRead!.Count)
            .Distinct()
            .OrderByDescending(c => c);

    var memberBookRankings = new Dictionary<int, int>();
    var rank = 1;
    foreach (var count in memberBookCounts)
    {
      memberBookRankings[count] = rank;
      rank += 1;
    }

    return memberBookRankings;
  }

  public Dictionary<int, int> CalculateBookRanks(List<Book> books)
  {
    var bookCounts = books
      .Select(m => m.MembersWhoHaveRead!.Count)
      .Distinct()
      .OrderByDescending(c => c);

    var bookRankings = new Dictionary<int, int>();
    var rank = 1;
    foreach (var count in bookCounts)
    {
      bookRankings[count] = rank;
      rank += 1;
    }

    return bookRankings;
  }


  public async Task OnGet()
  {
    var usersInMemberRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.MEMBERS);
    var usersInAlumniRole = await _userManager.GetUsersInRoleAsync(AuthConstants.Roles.ALUMNI);

    var memberUserIds = usersInMemberRole.Select(x => x.Id).ToList();
    var alumniUserIds = usersInAlumniRole.Select(x => x.Id).ToList();

    var memberSpec = new MembersHavingUserIdsWithBooksSpec(memberUserIds);
    var members = await _memberRepository.ListAsync(memberSpec);

    var alumniSpec = new MembersHavingUserIdsWithBooksSpec(alumniUserIds);
    var alumni = await _memberRepository.ListAsync(alumniSpec);

    Members = members
        .Where(m => (m.BooksRead?.Count ?? 0) > 0 &&
              !alumni.Any(alumni => alumni.Id == m.Id))
        .Select(member => MemberLinksDTO.FromMemberEntity(member))
        .ToList();

    var memberRanks = _rankingService.Rank(Members.Select(m => m.BooksRead!.Count));
    Members.ForEach(m => m.Rank = memberRanks[m.BooksRead!.Count]);

    Alumni = alumni.Select(alumni => MemberLinksDTO.FromMemberEntity(alumni))
        .Where(m => (m.BooksRead?.Count ?? 0) > 0)
        .ToList();

    var alumniRanks = _rankingService.Rank(Alumni.Select(m => m.BooksRead!.Count));
    Alumni.ForEach(m => m.Rank = alumniRanks[m.BooksRead!.Count]);

    var bookSpec = new BooksByMemberReadCountWithMembersWhoHaveReadSpec();
    Books = await _bookRepository.ListAsync(bookSpec);

    BookRanks = _rankingService.Rank(Books.Select(b => b.MembersWhoHaveRead!.Count));
  }

  public class MemberLinksDTO
  {
    public string? UserId { get; set; }
    public string? FullName { get; set; }
    public List<Book>? BooksRead { get; private set; }
    public int Rank { get; set; }

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
