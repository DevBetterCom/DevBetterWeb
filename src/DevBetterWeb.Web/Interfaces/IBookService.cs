using System.Threading.Tasks;
using DevBetterWeb.Web.Pages.Leaderboard;

namespace DevBetterWeb.Web.Interfaces;

/// <summary>
/// Interface for the BookService.
/// </summary>
public interface IBookService
{
	Task<BookDetailsViewModel?> GetBookByIdAsync(int bookId);
}
