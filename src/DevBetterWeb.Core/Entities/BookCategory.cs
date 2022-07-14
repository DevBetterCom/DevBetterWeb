using System.Collections.Generic;
using System.Linq;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Services;
using DevBetterWeb.Core.SharedKernel;

namespace DevBetterWeb.Core.Entities;

public class BookCategory : BaseEntity, IAggregateRoot
{
  public string? Title { get; set; }
  public List<Book> Books { get; set; } = new List<Book>();

  public void CalcAndSetBooksRank(RankingService<int> rankingService)
  {
		Book.CalcAndSetRank(rankingService, Books);
  }

  public void CalcAndSetMembersRank(RankingService<int> rankingService)
  {
	  Member.CalcAndSetBooksRank(rankingService, Books.SelectMany(x => x.MembersWhoHaveRead!).ToList());
  }

	public static void CalcAndSetCategoriesBooksRank(RankingService<int> rankingService, List<BookCategory> bookCategories)
  {
	  foreach (var bookCategory in bookCategories)
	  {
		  bookCategory.CalcAndSetBooksRank(rankingService);
	  }
  }

  public static void CalcAndSetMemberCategoriesMembersRank(RankingService<int> rankingService, List<BookCategory> bookCategories)
  {
	  foreach (var bookCategory in bookCategories)
	  {
		  bookCategory.CalcAndSetMembersRank(rankingService);
	  }
  }

	

	public override string ToString()
  {
    return Title + string.Empty;
  }
}
