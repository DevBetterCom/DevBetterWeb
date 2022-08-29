using System.Collections.Generic;

namespace DevBetterWeb.Web.Models;

public class MemberForBookDto
{
	public int Id { get; set; }
	public string? UserId { get; set; }
	public string? FullName { get; set; }
	public List<BookDto>? BooksRead { get; set; }
	public int BooksRank { get; set; }
	public int BooksReadCount { get; set; }
	public string? RoleName { get; set; }
}
