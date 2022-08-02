using System.Collections.Generic;
using System.Linq;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class Book : BaseEntity, IAggregateRoot
{
  public string? Title { get; set; }
  public string? Author { get; set; }
  public string? Details { get; set; }
  public string? PurchaseUrl { get; set; }
  public int? BookCategoryId { get; set; }
  public int Rank { get; private set; }
	public List<Member>? MembersWhoHaveRead { get; set; } = new List<Member>();
  public BookCategory? BookCategory { get; set; }

  public static void CalcAndSetRank(RankingService<int> rankingService, List<Book> books)
  {
	  var bookRanks = rankingService.Rank(books.Select(m => m.MembersWhoHaveRead!.Count));
	  books.ForEach(m => m.Rank = bookRanks[m.MembersWhoHaveRead!.Count]);
  }

	public override string ToString()
  {
    return Title + " by " + Author;
  }
}
