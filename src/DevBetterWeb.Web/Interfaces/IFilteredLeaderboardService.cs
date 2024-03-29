﻿using DevBetterWeb.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using DevBetterWeb.Web.Pages.Leaderboard;

namespace DevBetterWeb.Web.Interfaces;

public interface IFilteredLeaderboardService
{
	/// <summary>
	/// Removes non-current members from the leaderboard.
	/// </summary>
	/// <param name="bookCategories">The list of book categories in the leaderboard.</param>
	/// <param name="cancellationToken">An optional token to cancel the operation.</param>
	/// <returns>A new list of book categories with non-current members removed.</returns>
	Task<List<BookCategoryDto>> RemoveNonCurrentMembersFromLeaderBoardAsync(List<BookCategoryDto> bookCategories,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Removes non-current members from the book details view model.
	/// </summary>
	/// <param name="bookDetailsViewModel">A book in the book details.</param>
	/// <param name="cancellationToken">An optional token to cancel the operation.</param>
	/// <returns>A book details with non-current members removed.</returns>
	Task<BookDetailsViewModel> RemoveNonCurrentMembersFromBookDetailsAsync(BookDetailsViewModel bookDetailsViewModel,
		CancellationToken cancellationToken = default);
}
