using System.Collections.Generic;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.Interfaces;

/// <summary>
/// Provides methods for ranking members and books.
/// </summary>
public interface IRankingService
{
	/// <summary>
	/// Assigns a rank to each item in a collection.
	/// </summary>
	/// <typeparam name="TKey">The type of items in the collection.</typeparam>
	/// <param name="items">The collection of items to be ranked.</param>
	/// <returns>A dictionary where the keys are the items and the values are their ranks.</returns>
	Dictionary<TKey, int> Rank<TKey>(IEnumerable<TKey> items) where TKey : notnull;

	/// <summary>
	/// Calculates the book reading rank for each member.
	/// </summary>
	/// <param name="members">The list of members for which to calculate the book reading rank.</param>
	void CalculateMemberRank(List<MemberForBookDto> members);

	/// <summary>
	/// Calculates the popularity rank for each book.
	/// </summary>
	/// <param name="books">The list of books for which to calculate the popularity rank.</param>
	void CalculateBookRank(List<BookDto> books);

	/// <summary>
	/// Orders a list of members by their book reading rank.
	/// </summary>
	/// <param name="members">The list of members to be ordered.</param>
	/// <returns>A list of members ordered by their book reading rank.</returns>
	List<MemberForBookDto> OrderMembersByRank(List<MemberForBookDto> members);

	/// <summary>
	/// Orders a list of books by their popularity rank.
	/// </summary>
	/// <param name="books">The list of books to be ordered.</param>
	/// <returns>A list of books ordered by their popularity rank.</returns>
	List<BookDto> OrderBooksByRank(List<BookDto> books);
}

