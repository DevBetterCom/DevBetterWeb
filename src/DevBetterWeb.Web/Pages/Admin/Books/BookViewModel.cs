using System.ComponentModel.DataAnnotations;

namespace DevBetterWeb.Web.Pages.Admin.Books;

public class BookViewModel
{
  public int Id { get; set; }
  public string? Title { get; set; }
  public string? Author { get; set; }
  public string? Details { get; set; }

	[Display(Name = "Purchase Url")]
	public string? PurchaseUrl { get; set; }

	[Display(Name = "Uploaded By")]
  public string? MemberWhoUploaded { get; set; }
}
