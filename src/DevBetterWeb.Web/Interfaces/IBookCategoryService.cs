using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.Interfaces;

/// <summary>
/// Interface for the BookCategoryService.
/// </summary>
public interface IBookCategoryService
{
	Task<List<BookCategoryDto>> GetBookCategoriesAsync();
}
