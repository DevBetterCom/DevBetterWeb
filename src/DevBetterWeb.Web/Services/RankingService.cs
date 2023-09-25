using System.Collections.Generic;
using System.Linq;
using DevBetterWeb.Web.Interfaces;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.Services;

/// <summary>
/// Service for ranking operations, including member and book ranking.
/// </summary>
public class RankingService : IRankingService
{
	/// <summary>
	/// Assigns a rank to each item in a collection.
	/// </summary>
	/// <typeparam name="TKey">The type of items in the collection.</typeparam>
	/// <param name="items">The collection of items to be ranked.</param>
	/// <returns>A dictionary where the keys are the items and the values are their ranks.</returns>
	public Dictionary<TKey, int> Rank<TKey>(IEnumerable<TKey> items) where TKey : notnull
	{
		var result = new Dictionary<TKey, int>();

		var counts = items
			.Distinct()
			.OrderByDescending(i => i);

		var rank = 1;
		foreach (var count in counts)
		{
			result[count] = rank;
			rank += 1;
		}

		return result;
	}

	/// <summary>
	/// Calculates the book reading rank for each member.
	/// </summary>
	/// <param name="members">The list of members for which to calculate the book reading rank.</param>
	public void CalculateMemberRank(List<MemberForBookDto> members)
	{
		var bookCounts = members.Select(m => m.BooksReadCountByCategory).ToList();

		var memberRanks = Rank(bookCounts);
		foreach (var member in members)
		{
			member.BooksRank = memberRanks[member.BooksReadCountByCategory];
		}
	}

	/// <summary>
	/// Calculates the popularity rank for each book.
	/// </summary>
	/// <param name="books">The list of books for which to calculate the popularity rank.</param>
	public void CalculateBookRank(List<BookDto> books)
	{
		var readCounts = books.Select(m => m.MembersWhoHaveReadCount).ToList();

		var bookRanks = Rank(readCounts);
		books.ForEach(m => m.Rank = bookRanks[m.MembersWhoHaveReadCount]);
	}

	/// <summary>
	/// Orders a list of members by their book reading rank.
	/// </summary>
	/// <param name="members">The list of members to be ordered.</param>
	/// <returns>A list of members ordered by their book reading rank.</returns>
	public List<MemberForBookDto> OrderMembersByRank(List<MemberForBookDto> members)
	{
		return members.OrderBy(m => m.BooksRank).ToList();
	}

	/// <summary>
	/// Orders a list of books by their popularity rank.
	/// </summary>
	/// <param name="books">The list of books to be ordered.</param>
	/// <returns>A list of books ordered by their popularity rank.</returns>
	public List<BookDto> OrderBooksByRank(List<BookDto> books)
	{
		return books.OrderBy(b => b.Rank).ToList();
	}
}
