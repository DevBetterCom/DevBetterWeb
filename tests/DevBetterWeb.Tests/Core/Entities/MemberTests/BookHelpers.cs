using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Tests.Core.Entities.MemberTests
{
    public static class BookHelpers
    {
        public static Book GetDefaultBook()
        {
            return new Book
            {
                Title = "Book",
                Author = "Ilyana Smith",
                Details = "this is a book",
                PurchaseUrl = "https://buyabook.com"
            };
        }


    }
}
