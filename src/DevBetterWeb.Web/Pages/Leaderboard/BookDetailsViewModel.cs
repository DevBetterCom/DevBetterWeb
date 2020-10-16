using System.Collections.Generic;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Pages.Leaderboard
{
    public class BookDetailsViewModel
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? BookString { get; set; }
        public string? PurchaseUrl { get; set; }
        public string? Details { get; set; }
    public List<Member> MembersWhoHaveRead { get; set; } = new List<Member>();

        public BookDetailsViewModel()
        {
        }

        public BookDetailsViewModel(Book book)
        {
      Title = book.Title;
      Author = book.Author;
      BookString = book.ToString();
      PurchaseUrl = book.PurchaseUrl;
      Details = book.Details;
      MembersWhoHaveRead = book.MembersWhoHaveRead!;

        }
    }
}
