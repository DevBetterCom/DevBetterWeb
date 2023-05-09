using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.Interfaces;

public interface IBookCategoryService
{
	Task<List<BookCategoryDto>> GetBookCategoriesAsync();
}
