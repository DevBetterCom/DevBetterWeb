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

  public void ExcludeMembers(List<int> excludedMembersIds)
  {
	  foreach (var book in Books)
	  {
		  var newMembers = book.MembersWhoHaveRead!.Where(memberWhoHaveRead => !excludedMembersIds.Contains(memberWhoHaveRead.Id)).ToList();
		  book.MembersWhoHaveRead = newMembers.OrderBy(x => x.BooksRank).ToList();
		}
  }

	public static void CalcAndSetCategoriesBooksRank(RankingService<int> rankingService, List<BookCategory> bookCategories)
  {
	  foreach (var bookCategory in bookCategories)
	  {
		  bookCategory.CalcAndSetBooksRank(rankingService);
			bookCategory.Books = bookCategory.Books.OrderBy(x => x.Rank).ToList();
		}
	}

  public static void CalcAndSetMemberCategoriesMembersRank(RankingService<int> rankingService, List<BookCategory> bookCategories)
  {
	  foreach (var bookCategory in bookCategories)
	  {
		  bookCategory.CalcAndSetMembersRank(rankingService);
			bookCategory.Books = bookCategory.Books.OrderBy(x => x.Rank).ToList();
		}
  }

  public static void ExcludeMembers(List<BookCategory> bookCategories, List<int> excludedMembersIds)
  {
	  foreach (var bookCategory in bookCategories)
	  {
		  bookCategory.ExcludeMembers(excludedMembersIds);
	  }
	}


  public override string ToString()
  {
    return Title + string.Empty;
  }
}
