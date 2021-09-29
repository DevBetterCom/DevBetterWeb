using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.UnitTests.Core.Entities
{
  public static class BookHelpers
  {
    public static Book GetDefaultTestBook()
    {
      return new Book
      {
        Title = "Book",
        Author = "Ilyana Smith",
        Details = "this is a book",
        PurchaseUrl = "https://buyabook.com"
      };
    }
    public static Book GetDefaultTestBookWithSpecifiedTitle(string title)
    {
      return new Book
      {
        Title = title,
        Author = "Ilyana Smith",
        Details = "this is a book",
        PurchaseUrl = "https://buyabook.com"
      };
    }
  }
}

