using System.Collections.Generic;

namespace DevBetterWeb.Web.Models;

public class MemberForBookDto
{
	public string? UserId { get; set; }
	public string? FullName { get; set; }
	public List<BookDto>? BooksRead { get; set; }
	public int BooksRank { get; set; }
	public int BooksReadCount { get; set; }
}
