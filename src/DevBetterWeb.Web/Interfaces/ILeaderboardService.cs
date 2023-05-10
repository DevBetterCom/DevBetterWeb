using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Pages.Leaderboard;

namespace DevBetterWeb.Web.Interfaces;

/// <summary>
/// Interface for the LeaderboardService.
/// </summary>
public interface ILeaderboardService
{
	Task<List<BookCategoryDto>> SetBookCategoriesAsync();
	Task<BookDetailsViewModel?> GetBookDetailsAsync(string bookId);
}
