namespace DevBetterWeb.Web.Models;

public class BookDto
{
	public int? Id { get; set; }
	public string? Title { get; set; }
	public string? Author { get; set; }
	public string? Details { get; set; }
	public string? PurchaseUrl { get; set; }
	public int BookCategoryId { get; set; }
	public string? CategoryTitle { get; set; }
	public int MembersWhoHaveReadCount { get; set; }

	public override string ToString()
	{
		return Title + " by " + Author;
	}
}
