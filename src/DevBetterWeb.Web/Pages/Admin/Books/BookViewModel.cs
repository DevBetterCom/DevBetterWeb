using System.ComponentModel.DataAnnotations;

namespace DevBetterWeb.Web.Pages.Admin.Books;

public class BookViewModel
{
  public int Id { get; set; }

	[Required]
  public string? Title { get; set; }

	[Required]
	public string? Author { get; set; }
	[Display(Name = "Category")]
	public string? CategoryTitle { get; set; }

	[Required]
	[Range(1, 100, ErrorMessage = "Book Category is required")]
	[Display(Name = "Book Category")]
	public int? BookCategoryId { get; set; }

	[StringLength(2000, ErrorMessage = "Details cannot be longer than 2000 characters.")]
	public string? Details { get; set; }
	public int? MemberWhoUploadId { get; set; }

	[Display(Name = "Purchase Url")]
	public string? PurchaseUrl { get; set; }

	[Display(Name = "Uploaded By")]
  public string? MemberWhoUploaded { get; set; }
}
