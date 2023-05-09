using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.Interfaces;

public interface IRankAndOrderService
{
	Task UpdateRanksAndReadBooksCountForMemberAsync(List<BookCategoryDto> bookCategories);
	void UpdateMembersReadRank(List<BookCategoryDto> bookCategories);
	void UpdateBooksRank(List<BookCategoryDto> bookCategories);
	void OrderByRankForMembersAndBooks(List<BookCategoryDto> bookCategories);
}
