using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.Interfaces;

/// <summary>
/// Interface for the LeaderboardService.
/// </summary>
public interface ILeaderboardService
{
	Task<List<BookCategoryDto>> SetBookCategoriesAsync();
}
