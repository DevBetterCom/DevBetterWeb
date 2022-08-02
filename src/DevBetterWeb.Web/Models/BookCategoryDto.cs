using System.Collections.Generic;

namespace DevBetterWeb.Web.Models;

public class BookCategoryDto
{
	public int? Id { get; set; }
	public string? Title { get; set; }

	public List<MemberForBookDto> Members { get; set; } = new List<MemberForBookDto>();
	public List<BookDto>? Books { get; set; } = new List<BookDto>();
}
