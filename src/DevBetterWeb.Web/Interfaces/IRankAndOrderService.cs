using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.Interfaces;

/// <summary>
/// Interface for the RankAndOrderService.
/// </summary>
public interface IRankAndOrderService
{
	/// <summary>
	/// Asynchronously updates ranks and the count of books read for each member.
	/// </summary>
	/// <param name="bookCategories">The list of book categories.</param>
	Task UpdateRanksAndReadBooksCountForMemberAsync(List<BookCategoryDto> bookCategories);

	/// <summary>
	/// Updates the reading ranks of members.
	/// </summary>
	/// <param name="bookCategories">The list of book categories.</param>
	void UpdateMembersReadRank(List<BookCategoryDto> bookCategories);

	/// <summary>
	/// Updates the ranks of books.
	/// </summary>
	/// <param name="bookCategories">The list of book categories.</param>
	void UpdateBooksRank(List<BookCategoryDto> bookCategories);

	/// <summary>
	/// Orders members and books by rank.
	/// </summary>
	/// <param name="bookCategories">The list of book categories.</param>
	void OrderByRankForMembersAndBooks(List<BookCategoryDto> bookCategories);
}
